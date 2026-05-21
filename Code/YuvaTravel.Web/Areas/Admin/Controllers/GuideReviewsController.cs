using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Guides;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class GuideReviewsController : BaseController
    {
        public GuideReviewsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.GuideReview)]
        public async Task<IActionResult> Index(GuideReviewFilter filter)
        {
            ViewBag.GuideSelectList = await GetGuideSelectList(filter.GuideId);
            if (filter.GuideId.HasValue)
                ViewBag.CurrentGuide = await uow.GuideRepo.FindAsync(filter.GuideId);

            if (ModelState.IsValid)
                ViewBag.ListData = uow.GuideReviewRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<GuideReview>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.GuideReview)]
        public async Task<IActionResult> Create(Guid? guideId)
        {
            var dto = new GuideReviewCreateOrEdit
            {
                GuideId = guideId,
                DisplaySeq = uow.GuideReviewRepo.GetMaxDisplaySeq(guideId)
            };
            ViewBag.GuideSelectList = await GetGuideSelectList(guideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.GuideReview)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GuideReviewCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuideReview { GuideReviewId = Guid.NewGuid() };
                entity.InjectFrom(dto);
                entity.UserLog.CreateTime = DateTime.Now;
                entity.UserLog.CreateUserId = UserId;

                uow.GuideReviewRepo.Add(entity);
                await uow.CommitAsync();

                TempData["success-msg"] = $"{StrText.GuideReviews}{StrText.CreateSuccess}";
                return RedirectToAction("Index", new { guideId = dto.GuideId });
            }
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.GuideReview)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            var entity = await uow.GuideReviewRepo.FindAsync(id);
            if (entity == null) return NotFound();

            var dto = new GuideReviewCreateOrEdit();
            dto.InjectFrom(entity);
            dto.GuideReviewId = entity.GuideReviewId;
            dto.GuideId = entity.GuideId;
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.GuideReview)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GuideReviewCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var entity = await uow.GuideReviewRepo.FindAsync(dto.GuideReviewId, isANT: false);
                if (entity == null) return NotFound();
                entity.InjectFrom(dto);
                entity.UserLog.UpdateTime = DateTime.Now;
                entity.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.GuideReviews}{StrText.EditSuccess}";
                return RedirectToAction("Index", new { guideId = dto.GuideId });
            }
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.GuideReview)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await uow.GuideReviewRepo.FindAsync(id);
            if (entity == null) return NotFound();

            var dto = new GuideReviewCreateOrEdit();
            dto.InjectFrom(entity);
            dto.GuideReviewId = entity.GuideReviewId;
            dto.GuideId = entity.GuideId;
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.GuideReview)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var entity = await uow.GuideReviewRepo.FindAsync(id, isANT: false);
            if (entity == null) return NotFound();
            var guideId = entity.GuideId;
            uow.GuideReviewRepo.Delete(entity);
            await uow.CommitAsync();
            TempData["success-msg"] = $"{StrText.GuideReviews}{StrText.DeleteSuccess}";
            return RedirectToAction("Index", new { guideId });
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetGuideSelectList(Guid? selectedId)
        {
            return await uow.GuideRepo.All()
                .OrderBy(p => p.DisplaySeq)
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p.GuideId.ToString(),
                    Text = p.Name,
                    Selected = selectedId.HasValue && p.GuideId == selectedId.Value
                })
                .ToListAsync();
        }
    }
}
