using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.ArticleCategories;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class ArticleCategoriesController : BaseController
    {
        IUnitOfWork uow;

        public ArticleCategoriesController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            uow = unitOfWork;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.ArticleCategory)]
        public IActionResult Index(ArticleCategoryFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.ArticleCategoryRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<ArticleCategory>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.ArticleCategory)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            ArticleCategory articleCategory = await uow.ArticleCategoryRepo.FindAsync(id);   // 顯示用,ANT
            if (articleCategory == null) return NotFound();
            return View(articleCategory);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.ArticleCategory)]
        public IActionResult Create()
        {
            var dto = new ArticleCategoryCreateOrEdit { DisplaySeq = uow.ArticleCategoryRepo.GetMaxDisplaySeq() };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.ArticleCategory)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleCategoryCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var articleCategory = new ArticleCategory { ArticleCategoryId = Guid.NewGuid() };
                articleCategory.InjectFrom(dto);
                articleCategory.UserLog.CreateTime = DateTime.Now;
                articleCategory.UserLog.CreateUserId = UserId;

                uow.ArticleCategoryRepo.Add(articleCategory);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.ArticleCategories}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.ArticleCategory)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            ArticleCategory articleCategory = await uow.ArticleCategoryRepo.FindAsync(id);   // GET 顯示用,ANT
            if (articleCategory == null) return NotFound();
            var dto = new ArticleCategoryCreateOrEdit();
            dto.InjectFrom(articleCategory);
            dto.ArticleCategoryId = articleCategory.ArticleCategoryId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.ArticleCategory)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleCategoryCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                ArticleCategory articleCategory = await uow.ArticleCategoryRepo.FindAsync(dto.ArticleCategoryId, isANT: false);
                if (articleCategory == null) return NotFound();

                articleCategory.InjectFrom(dto);
                articleCategory.UserLog.UpdateTime = DateTime.Now;
                articleCategory.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.ArticleCategories}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.ArticleCategory)]
        public async Task<IActionResult> Delete(Guid id)
        {
            ArticleCategory articleCategory = await uow.ArticleCategoryRepo.FindAsync(id);
            if (articleCategory == null) return NotFound();
            return View(articleCategory);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.ArticleCategory)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Delete 用 tracking 較安全
            ArticleCategory articleCategory = await uow.ArticleCategoryRepo.FindAsync(id, isANT: false);
            if (articleCategory == null) return NotFound();
            uow.ArticleCategoryRepo.Delete(articleCategory);
            TempData["success-msg"] = $"{StrText.ArticleCategories}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
