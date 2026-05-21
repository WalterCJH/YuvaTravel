using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.ArticleComments;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    //[AuthorizeAdmin(new ProgramId[] { ProgramId.Article, ProgramId.ArticleReview })]
    [Area("Admin")]
    public class ArticleCommentsController : BaseController
    {
        public ArticleCommentsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleCommentCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var articleComment = new ArticleComment
                {
                    ArticleCommentId = Guid.NewGuid(),
                    ArticleId = dto.ArticleId,
                    CommentContent = dto.CommentContent,
                    CommentTime = DateTime.Now,
                    CommentUserId = UserId
                };

                uow.ArticleCommentRepo.Add(articleComment);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.ArticleComments}{StrText.CreateSuccess}";
                return RedirectToAction("Edit", "ArticleReviews", new { id = dto.ArticleId });
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleCommentCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                ArticleComment articleComment = await uow.ArticleCommentRepo.FindAsync(dto.ArticleCommentId, isANT: false);
                if (articleComment == null) return NotFound();
                articleComment.CommentContent = dto.CommentContent;
                articleComment.CommentTime = DateTime.Now;
                articleComment.CommentUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.ArticleComments}{StrText.EditSuccess}";
                return RedirectToAction("Edit", "ArticleReviews", new { id = dto.ArticleId });
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid ArticleCommentId, Guid ArticleId)
        {
            ArticleComment articleComment = await uow.ArticleCommentRepo.FindAsync(ArticleCommentId, isANT: false);
            if (articleComment == null) return NotFound();
            uow.ArticleCommentRepo.Delete(articleComment);
            await uow.CommitAsync();
            TempData["success-msg"] = $"{StrText.ArticleComments}{StrText.DeleteSuccess}";
            return RedirectToAction("Edit", "ArticleReviews", new { id = ArticleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(ArticleCommentCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // Reply 也是改 entity → 要 tracking
                ArticleComment articleComment = await uow.ArticleCommentRepo.FindAsync(dto.ArticleCommentId, isANT: false);
                if (articleComment == null) return NotFound();
                articleComment.ReplyContent = dto.ReplyContent;
                articleComment.ReplyTime = DateTime.Now;
                articleComment.ReplyUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.ArticleComments}{StrText.ReplySuccess}";
                return RedirectToAction("Edit", "Articles", new { id = articleComment.ArticleId });
            }

            return View(dto);
        }
    }
}
