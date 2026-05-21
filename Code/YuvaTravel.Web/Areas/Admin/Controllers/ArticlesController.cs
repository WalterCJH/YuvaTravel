using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Articles;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Storage;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Web.Helpers;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class ArticlesController : BaseController
    {
        IWebHostEnvironment _env;
        IImageUploadService _imgUpload;

        public ArticlesController(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
            IImageUploadService imageUploadService)
            : base(unitOfWork)
        {
            _env = env;
            _imgUpload = imageUploadService;
        }

        private async Task PopulateAuthorSelectList(Guid? selectedId = null)
        {
            var authors = await uow.AuthorRepo.IncludeAll()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplaySeq).ThenBy(p => p.Name)
                .Select(p => new { p.AuthorId, p.Name, GuideName = p.Guide != null ? p.Guide.Name : null })
                .ToListAsync();
            ViewBag.AuthorSelectList = authors.Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = a.AuthorId.ToString(),
                Text = string.IsNullOrEmpty(a.GuideName) ? a.Name : $"{a.Name} (嚮導:{a.GuideName})",
                Selected = selectedId.HasValue && a.AuthorId == selectedId.Value
            }).ToList();
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.Article)]
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
        [AuthorizeAdmin(ActionMode.Details, ProgramId.Article)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            Article article = await uow.ArticleRepo.FindAsync(id);
            if (article == null) return NotFound();
            article.TagIds = uow.ArticleTagRepo.QueryTagFromArticle(id.Value);
            return View(article);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Article)]
        [ViewBagArticleCategoryId]
        public async Task<IActionResult> Create()
        {
            var dto = new ArticleCreateOrEdit
            {
                DisplaySeq = uow.ArticleRepo.GetMaxDisplaySeq()
            };
            dto.SettingArticleCategory(await uow.ArticleCategoryRepo.All().ToListAsync());
            dto.SettingRegions(await uow.RegionRepo.All().ToListAsync());
            await PopulateAuthorSelectList();
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Article)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagArticleCategoryId]
        public async Task<IActionResult> Create(ArticleCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var article = new Article { ArticleId = Guid.NewGuid() };
                article.InjectFrom(dto);

                // 必須先 Add Article 再做 junction (ArticleCategoryArticle / ArticleRegion),
                // 否則 commit 時 junction rows 會帶著「DB 還沒有的 ArticleId」先寫入 → FK 條件約束失敗
                uow.ArticleRepo.Add(article);

                await uow.ArticleCategoryArticleRepo.UpdateArticleCategoryArticle(dto.ArticleCategories, article.ArticleId, UserId);
                await uow.ArticleRegionRepo.UpdateArticleRegions(dto.Regions, article.ArticleId, UserId);

                article.ArticleType = dto.ArticleType.Value;
                article.Code = article.Code?.Trim();

                if (dto.Content?.Length > 24000)
                    article.Content4 = dto.Content.Substring(24000);
                if (dto.Content?.Length > 16000)
                    article.Content3 = (dto.Content?.Length > 24000) ? dto.Content.Substring(16000, 8000) : dto.Content.Substring(16000);
                if (dto.Content?.Length > 8000)
                {
                    article.Content2 = (dto.Content?.Length > 16000) ? dto.Content.Substring(8000, 8000) : dto.Content.Substring(8000);
                    article.Content = dto.Content.Substring(0, 8000);
                }

                article.UserLog.CreateTime = DateTime.Now;
                article.UserLog.CreateUserId = UserId;

                uow.TagRepo.CreateTag(dto.TagIds, UserId);
                uow.Commit();   // 新 Tag + Article + junction 一次寫入,FK 滿足

                uow.ArticleTagRepo.UpdateArticleTagFromArticle(article.ArticleId, dto.TagIds);

                var serialNumber = await uow.ArticleRepo.GetMaxSerialNumber();

                if (!string.IsNullOrWhiteSpace(dto.ImageFileBase64))
                {
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageFileName ?? "image");
                    var folder = new[] { Folder.Articles, serialNumber.ToString() };

                    article.ImagePath         = await _imgUpload.UploadAsync(dto.ImageFileBase64, ImageSize.ArticleWidth,         folder, baseName);
                    article.ImageHomePath     = await _imgUpload.UploadAsync(dto.ImageFileBase64, ImageSize.ArticleHomeWidth,     folder, baseName + "-h");
                    article.ImageCategoryPath = await _imgUpload.UploadAsync(dto.ImageFileBase64, ImageSize.ArticleCategoryWidth, folder, baseName + "-c");
                    article.ImageMobilePath   = await _imgUpload.UploadAsync(dto.ImageFileBase64, ImageSize.ArticleMobileWidth,   folder, baseName + "-m");
                }

                if (!string.IsNullOrWhiteSpace(dto.ImageMobileFileBase64))
                {
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageMobileFileName ?? "image");
                    var folder = new[] { Folder.Articles, serialNumber.ToString() };
                    article.ImageBannerMobilePath = await _imgUpload.UploadAsync(dto.ImageMobileFileBase64, ImageSize.BannerMobileWidth, folder, baseName + "-banner");
                }

                int seq = 1;
                foreach (var item in dto.ArticleSDCommonQuestions)
                {
                    var articleSDCommonQuestion = new ArticleSDCommonQuestion
                    {
                        ArticleSDCommonQuestionId = item.ArticleSDQuestionId.Value,
                        ArticleId = article.ArticleId,
                        Name = item.Name,
                        Text = item.Text,
                        DisplaySequence = seq++
                    };
                    uow.ArticleSDCommonQuestionRepo.Add(articleSDCommonQuestion);
                }

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Articles}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            await PopulateAuthorSelectList(dto.AuthorId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Article)]
        [ViewBagArticleCategoryId]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            Article article = await uow.ArticleRepo.FindAsync(id);
            if (article == null) return NotFound();

            var dto = new ArticleCreateOrEdit();
            dto.InjectFrom(article);
            dto.ArticleId = article.ArticleId;
            dto.ArticleType = article.ArticleType;
            dto.Content = article.ContentAll;
            dto.ImagePath_ = article.ImagePath;
            dto.ImageBannerMobilePath_ = article.ImageBannerMobilePath;
            dto.NotReplyCount = article.NotReplyCount;
            dto.ArticleComments = uow.ArticleCommentRepo.FindFromArticleId(dto.ArticleId.Value, UserId);
            dto.TagIds = uow.ArticleTagRepo.QueryTagFromArticle(dto.ArticleId.Value);
            dto.ArticleSDCommonQuestions = uow.ArticleSDCommonQuestionRepo.QueryFromArticleId(dto.ArticleId.Value);
            var articleCategoryIds = article.ArticleCategoryArticles.Select(c => c.ArticleCategoryId).ToList();
            dto.SettingArticleCategory(await uow.ArticleCategoryRepo.All().ToListAsync(), articleCategoryIds);
            var regionIds = article.ArticleRegions?.Select(r => r.RegionId).ToList() ?? new List<Guid>();
            dto.SettingRegions(await uow.RegionRepo.All().ToListAsync(), regionIds);
            await PopulateAuthorSelectList(dto.AuthorId);

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Article)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagArticleCategoryId]
        public async Task<IActionResult> Edit(ArticleCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                Article article = await uow.ArticleRepo.FindAsync(dto.ArticleId, isANT: false);
                if (article == null) return NotFound();
                article.InjectFrom(dto);
                article.ArticleType = dto.ArticleType.Value;
                article.Code = article.Code?.Trim();

                await uow.ArticleCategoryArticleRepo.UpdateArticleCategoryArticle(dto.ArticleCategories, article.ArticleId, UserId);
                await uow.ArticleRegionRepo.UpdateArticleRegions(dto.Regions, article.ArticleId, UserId);

                if (dto.Content?.Length > 24000)
                    article.Content4 = dto.Content.Substring(24000);
                if (dto.Content?.Length > 16000)
                    article.Content3 = (dto.Content?.Length > 24000) ? dto.Content.Substring(16000, 8000) : dto.Content.Substring(16000);
                if (dto.Content?.Length > 8000)
                {
                    article.Content2 = (dto.Content?.Length > 16000) ? dto.Content.Substring(8000, 8000) : dto.Content.Substring(8000);
                    article.Content = dto.Content.Substring(0, 8000);
                }

                article.UserLog.UpdateTime = DateTime.Now;
                article.UserLog.UpdateUserId = UserId;

                uow.TagRepo.CreateTag(dto.TagIds, UserId);
                await uow.CommitAsync();   // 新 Tag 先 commit 拿 ID

                uow.ArticleTagRepo.UpdateArticleTagFromArticle(article.ArticleId, dto.TagIds);

                if (!string.IsNullOrWhiteSpace(dto.ImageFileBase64))
                {
                    await _imgUpload.DeleteAsync(article.ImagePath);
                    await _imgUpload.DeleteAsync(article.ImageHomePath);
                    await _imgUpload.DeleteAsync(article.ImageCategoryPath);
                    await _imgUpload.DeleteAsync(article.ImageMobilePath);

                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageFileName ?? "image");
                    var folder = new[] { Folder.Articles, article.SerialNumber.ToString() };

                    article.ImagePath         = await _imgUpload.UploadAsync(dto.ImageFileBase64, ImageSize.ArticleWidth,         folder, baseName);
                    article.ImageHomePath     = await _imgUpload.UploadAsync(dto.ImageFileBase64, ImageSize.ArticleHomeWidth,     folder, baseName + "-h");
                    article.ImageCategoryPath = await _imgUpload.UploadAsync(dto.ImageFileBase64, ImageSize.ArticleCategoryWidth, folder, baseName + "-c");
                    article.ImageMobilePath   = await _imgUpload.UploadAsync(dto.ImageFileBase64, ImageSize.ArticleMobileWidth,   folder, baseName + "-m");
                }

                if (!string.IsNullOrWhiteSpace(dto.ImageMobileFileBase64))
                {
                    await _imgUpload.DeleteAsync(article.ImageBannerMobilePath);
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageMobileFileName ?? "image");
                    var folder = new[] { Folder.Articles, article.SerialNumber.ToString() };
                    article.ImageBannerMobilePath = await _imgUpload.UploadAsync(dto.ImageMobileFileBase64, ImageSize.BannerMobileWidth, folder, baseName + "-banner");
                }

                // SD 問答先清空再重建
                var articleSDCQs = await uow.ArticleSDCommonQuestionRepo.QueryEntityFromArticleId(article.ArticleId, isANT: false);
                foreach (var item in articleSDCQs)
                    uow.ArticleSDCommonQuestionRepo.Delete(item);

                int seq = 1;
                foreach (var item in dto.ArticleSDCommonQuestions)
                {
                    var articleSDCommonQuestion = new ArticleSDCommonQuestion
                    {
                        ArticleSDCommonQuestionId = Guid.NewGuid(),
                        ArticleId = article.ArticleId,
                        Name = item.Name,
                        Text = item.Text,
                        DisplaySequence = seq++
                    };
                    uow.ArticleSDCommonQuestionRepo.Add(articleSDCommonQuestion);
                }

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Articles}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }

            dto.ArticleComments = uow.ArticleCommentRepo.FindFromArticleId(dto.ArticleId.Value, UserId);
            await PopulateAuthorSelectList(dto.AuthorId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Article)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Article article = await uow.ArticleRepo.FindAsync(id);
            if (article == null) return NotFound();
            article.TagIds = uow.ArticleTagRepo.QueryTagFromArticle(id);
            return View(article);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Article)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid ArticleId)
        {
            Article article = await uow.ArticleRepo.FindAsync(ArticleId, isANT: false);
            if (article == null) return NotFound();
            uow.ArticleRepo.Delete(article);
            TempData["success-msg"] = $"{StrText.Articles}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.Article)]
        public async Task<IActionResult> Preview(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            Article? article = null;

            if (Guid.TryParse(id, out Guid articleId))
                article = await uow.ArticleRepo.FindAsync(articleId, isANT: false);
            else
                article = await uow.ArticleRepo.FindCodeAsync(id, isANT: false);

            if (article == null) return NotFound();

            GoRegister = StrPath.ClickUrl(Host, $"{WebConfig.GoRegister}{article.PromotionCode}");

            var dto = await DataGenHelper.GetArticles(uow.ArticleTagRepo, uow.ArticleSDCommonQuestionRepo, article, Host, GoRegister);
            dto.Author = article.Author?.Name;
            dto.AuthorBio = article.Author?.Bio;
            dto.GuideRegions = article.Author?.Guide?.GuideRegions?
                .OrderBy(r => r.Region.DisplaySeq)
                .Select(r => new GuideRegionRef { Name = r.Region.Name, Code = r.Region.Code })
                .ToList() ?? new List<GuideRegionRef>();
            dto.ReadTimeMin = article.ReadTimeMin;
            dto.SerialNumber = article.SerialNumber;
            dto.OnlineTime = article.OnlineTime;
            dto.Views = article.Views;

            var relatedRaw = await uow.ArticleRepo.ActiveArticleANT()
                .Where(p => p.ArticleId != article.ArticleId &&
                    p.ArticleCategoryArticles.Any(a => a.ArticleCategory.Code == dto.CategoryCode))
                .OrderByDescending(p => p.Views)
                .Take(3)
                .ToListAsync();
            dto.RelatedArticles = DataGenHelper.GetBaseArticlesFromArticles(relatedRaw, uow.ArticleTagRepo, Host, false, true);

            var prevRaw = await uow.ArticleRepo.ActiveArticleANT()
                .Where(p => p.OnlineTime < article.OnlineTime)
                .OrderByDescending(p => p.OnlineTime)
                .FirstOrDefaultAsync();
            var nextRaw = await uow.ArticleRepo.ActiveArticleANT()
                .Where(p => p.OnlineTime > article.OnlineTime)
                .OrderBy(p => p.OnlineTime)
                .FirstOrDefaultAsync();
            if (prevRaw != null)
                dto.PrevArticle = DataGenHelper.GetBaseArticlesFromArticles(new List<Article> { prevRaw }, uow.ArticleTagRepo, Host, false, false).First();
            if (nextRaw != null)
                dto.NextArticle = DataGenHelper.GetBaseArticlesFromArticles(new List<Article> { nextRaw }, uow.ArticleTagRepo, Host, false, false).First();

            // 注意:不可把 Article 實體放進 TempData。TempData 在請求結束時會用
            // BsonTempDataSerializer 序列化(因為專案啟用 AddNewtonsoftJson),
            // 無法序列化 EF 實體 → InvalidOperationException。
            // 前台 HomeController 之所以沒事,是因為前台 BaseController 會在同一個
            // 請求內讀取並清空 TempData["Article"];Admin 的 BaseController 沒有這段,
            // 預覽流程也不需要 meta,故此處不寫入 TempData。

            ViewBag.ArticleImagePath = dto.ImagePath;
            ViewBag.ArticleImageMobilePath = dto.ImageMobilePath;

            ViewBag.Preview = "預覽文章 - ";

            return View("~/Views/Home/Article.cshtml", dto);
        }

        private async Task<AsideSearchTagDto> GetAsideSearchTagDto(string code = null, string tag = null)
        {
            var dto = new AsideSearchTagDto();
            dto.HotTags = await uow.TagRepo.QueryHotTag(code, tag);
            return dto;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Export, ProgramId.Article)]
        public IActionResult Export()
        {
            string exportName = "文章";

            var workbook = new XLWorkbook();
            var wsSheet = workbook.Worksheets.Add(exportName);
            int row = 1;
            int col = 1;

            var columnName = new List<string> {
                "順序", "審核狀態", "觀看次數", "推廣碼", "文章作者",
                "文章類別", "文章代碼", "文章標題", "文章審核人員", "文章審核時間",
                "文章上線排程時間", "文章上線時間", "標籤", "精選文章", "精選順序"
            };

            foreach (var item in columnName)
            {
                wsSheet.Cell(row, col).Value = item;
                wsSheet.Cell(row, col).Style.Alignment.WrapText = true;
                col++;
            }

            var datas = uow.ArticleRepo.IncludeAll().OrderByDescending(c => c.DisplaySeq).ToList();

            foreach (var data in datas)
            {
                row++;
                col = 1;

                wsSheet.Cell(row, col++).Value = data.DisplaySeq;
                wsSheet.Cell(row, col++).Value = data.ArticleReviewType.ToString();
                wsSheet.Cell(row, col++).Value = data.Views;
                wsSheet.Cell(row, col++).Value = data.PromotionCode;
                wsSheet.Cell(row, col++).Value = data.Author?.Name ?? "";
                wsSheet.Cell(row, col++).Value = data.ArticleCategoryArticles.FirstOrDefault()?.ArticleCategory?.Name;
                wsSheet.Cell(row, col++).Value = data.Code;
                wsSheet.Cell(row, col++).Value = data.Title;
                wsSheet.Cell(row, col++).Value = data.ReviewUserId;
                wsSheet.Cell(row, col++).Value = data.ReviewTime?.ToString("yyyy/MM/dd HH:mm:ss");
                wsSheet.Cell(row, col++).Value = data.OnScheduleTime?.ToString("yyyy/MM/dd HH:mm:ss");
                wsSheet.Cell(row, col++).Value = data.OnlineTime?.ToString("yyyy/MM/dd HH:mm:ss");
                wsSheet.Cell(row, col++).Value = string.Join("、", data.ArticleTags.Select(p => p.Tag.Name).ToArray());
                wsSheet.Cell(row, col++).Value = (data.IsFeaturedArticle) ? "Y" : "";
                wsSheet.Cell(row, col++).Value = data.FeaturedArticleDisplaySeq;
            }

            wsSheet.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return File(ms.ToArray(), "application/excel", $"{exportName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
        }

        public PartialViewResult AddStructuredDataCommonQuestion(string T)
        {
            var dto = new ArticleSDCommonQuestionCreateOrEdit
            {
                ArticleSDQuestionId = Guid.NewGuid()
            };
            return PartialView(@"~\Areas\Admin\Views\Articles\_StructuredDataCommonQuestion.cshtml", dto);
        }
    }
}
