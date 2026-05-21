using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.FuncGroups;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class FuncGroupsController : BaseController
    {
        public FuncGroupsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.FuncGroup)]
        public IActionResult Index(FuncGroupFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.FuncGroupRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<FuncGroup>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.FuncGroup)]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return BadRequest();
            FuncGroup funcGroup = await uow.FuncGroupRepo.FindAsync(id);
            if (funcGroup == null) return NotFound();
            return View(funcGroup);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.FuncGroup)]
        public IActionResult Create()
        {
            var dto = new FuncGroupCreateOrEdit { DisplaySeq = uow.FuncGroupRepo.GetMaxDisplaySeq() };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.FuncGroup)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FuncGroupCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var funcGroup = new FuncGroup();
                funcGroup.InjectFrom(dto);
                funcGroup.UserLog.CreateTime = DateTime.Now;
                funcGroup.UserLog.CreateUserId = UserId;

                uow.FuncGroupRepo.Add(funcGroup);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.FuncGroups}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.FuncGroup)]
        [ViewBagSalutation]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return BadRequest();
            FuncGroup funcGroup = await uow.FuncGroupRepo.FindAsync(id);
            if (funcGroup == null) return NotFound();
            var dto = new FuncGroupCreateOrEdit();
            dto.InjectFrom(funcGroup);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.FuncGroup)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagSalutation]
        public async Task<IActionResult> Edit(FuncGroupCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                FuncGroup funcGroup = await uow.FuncGroupRepo.FindAsync(dto.FuncGroupId, isANT: false);
                if (funcGroup == null) return NotFound();
                funcGroup.InjectFrom(dto);
                funcGroup.UserLog.UpdateTime = DateTime.Now;
                funcGroup.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.FuncGroups}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.FuncGroup)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return BadRequest();
            FuncGroup funcGroup = await uow.FuncGroupRepo.FindAsync(id);
            if (funcGroup == null) return NotFound();
            return View(funcGroup);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.FuncGroup)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string FuncGroupId)
        {
            FuncGroup funcGroup = await uow.FuncGroupRepo.FindAsync(FuncGroupId, isANT: false);
            if (funcGroup == null) return NotFound();
            uow.FuncGroupRepo.Delete(funcGroup);
            TempData["success-msg"] = $"{StrText.FuncGroups}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
