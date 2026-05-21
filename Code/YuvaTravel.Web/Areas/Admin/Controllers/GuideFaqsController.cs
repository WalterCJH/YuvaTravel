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
    public class GuideFaqsController : BaseController
    {
        public GuideFaqsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.GuideFaq)]
        public async Task<IActionResult> Index(GuideFaqFilter filter)
        {
            ViewBag.GuideSelectList = await GetGuideSelectList(filter.GuideId);
            if (filter.GuideId.HasValue)
                ViewBag.CurrentGuide = await uow.GuideRepo.FindAsync(filter.GuideId);

            if (ModelState.IsValid)
                ViewBag.ListData = uow.GuideFaqRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<GuideFaq>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.GuideFaq)]
        public async Task<IActionResult> Create(Guid? guideId)
        {
            var dto = new GuideFaqCreateOrEdit
            {
                GuideId = guideId,
                DisplaySeq = uow.GuideFaqRepo.GetMaxDisplaySeq(guideId)
            };
            ViewBag.GuideSelectList = await GetGuideSelectList(guideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.GuideFaq)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GuideFaqCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuideFaq { GuideFaqId = Guid.NewGuid() };
                entity.InjectFrom(dto);
                entity.UserLog.CreateTime = DateTime.Now;
                entity.UserLog.CreateUserId = UserId;

                uow.GuideFaqRepo.Add(entity);
                await uow.CommitAsync();

                TempData["success-msg"] = $"{StrText.GuideFaqs}{StrText.CreateSuccess}";
                return RedirectToAction("Index", new { guideId = dto.GuideId });
            }
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.GuideFaq)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            var entity = await uow.GuideFaqRepo.FindAsync(id);
            if (entity == null) return NotFound();

            var dto = new GuideFaqCreateOrEdit();
            dto.InjectFrom(entity);
            dto.GuideFaqId = entity.GuideFaqId;
            dto.GuideId = entity.GuideId;
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.GuideFaq)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GuideFaqCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var entity = await uow.GuideFaqRepo.FindAsync(dto.GuideFaqId, isANT: false);
                if (entity == null) return NotFound();
                entity.InjectFrom(dto);
                entity.UserLog.UpdateTime = DateTime.Now;
                entity.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.GuideFaqs}{StrText.EditSuccess}";
                return RedirectToAction("Index", new { guideId = dto.GuideId });
            }
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.GuideFaq)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await uow.GuideFaqRepo.FindAsync(id);
            if (entity == null) return NotFound();

            var dto = new GuideFaqCreateOrEdit();
            dto.InjectFrom(entity);
            dto.GuideFaqId = entity.GuideFaqId;
            dto.GuideId = entity.GuideId;
            ViewBag.GuideSelectList = await GetGuideSelectList(dto.GuideId);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.GuideFaq)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var entity = await uow.GuideFaqRepo.FindAsync(id, isANT: false);
            if (entity == null) return NotFound();
            var guideId = entity.GuideId;
            uow.GuideFaqRepo.Delete(entity);
            await uow.CommitAsync();
            TempData["success-msg"] = $"{StrText.GuideFaqs}{StrText.DeleteSuccess}";
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
