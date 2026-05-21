using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.ArticleReviews;
using YuvaTravel.Data.Dtos.Articles;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class ArticleReviewsController : BaseController
    {
        public ArticleReviewsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.ArticleReview)]
        [ViewBagArticleCategoryId]
        [ViewBagAuthor]
        public IActionResult Index(ArticleFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.ArticleRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<Article>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.ArticleReview)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            Article article = await uow.ArticleRepo.FindAsync(id);
            if (article == null) return NotFound();

            var dto = new ArticleReviewEdit();
            dto.InjectFrom(article);
            dto.ArticleId = article.ArticleId;
            dto.ArticleCategoryArticles = article.ArticleCategoryArticles.ToList();
            dto.NotReplyCount = article.NotReplyCount;
            dto.ArticleComments = uow.ArticleCommentRepo.FindFromArticleId(dto.ArticleId, UserId);
            dto.TagIds = uow.ArticleTagRepo.QueryTagFromArticle(id.Value);

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.ArticleReview)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleReviewEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                Article article = await uow.ArticleRepo.FindAsync(dto.ArticleId, isANT: false);
                if (article == null) return NotFound();
                article.IsTop = dto.IsTop;
                article.IsFeaturedArticle = dto.IsFeaturedArticle;
                article.FeaturedArticleDisplaySeq = dto.FeaturedArticleDisplaySeq;
                article.ArticleReviewType = dto.ArticleReviewType;
                article.DisplaySeq = dto.DisplaySeq;
                if ((article.ArticleReviewType == ArticleReviewType.已通過 || article.ArticleReviewType == ArticleReviewType.排程中) && article.ReviewTime == null)
                {
                    article.ReviewTime = DateTime.Now; // ReviewTime 要在 OnlineTime 之前給值
                }
                article.ReviewUserId = UserId;
                article.OnScheduleTime = dto.OnScheduleTime;
                article.OnlineTime = (article.OnScheduleTime > article.ReviewTime) ? article.OnScheduleTime : article.ReviewTime;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.ArticleReviews}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }

            dto.ArticleComments = uow.ArticleCommentRepo.FindFromArticleId(dto.ArticleId, UserId);
            return View(dto);
        }
    }
}
