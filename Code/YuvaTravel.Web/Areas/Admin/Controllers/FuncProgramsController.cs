using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.FuncPrograms;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class FuncProgramsController : BaseController
    {
        public FuncProgramsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.FuncProgram)]
        [ViewBagFuncGroupId]
        [ViewBagAuthorizeLevel]
        public IActionResult Index(FuncProgramFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.FuncProgramRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<FuncProgram>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.FuncProgram)]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return BadRequest();
            FuncProgram funcProgram = await uow.FuncProgramRepo.FindAsync(id);
            if (funcProgram == null) return NotFound();
            var dto = new FuncProgramDetailsOrDelete();
            dto.InjectFrom(funcProgram);
            dto.FuncGroupName = funcProgram.FuncGroup.Name;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.FuncProgram)]
        [ViewBagFuncGroupId]
        [ViewBagProgramId]
        [ViewBagAuthorizeLevel]
        public IActionResult Create()
        {
            var dto = new FuncProgramCreateOrEdit { DisplaySeq = uow.FuncProgramRepo.GetHundredMaxDisplaySeq() };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.FuncProgram)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagFuncGroupId]
        [ViewBagProgramId]
        [ViewBagAuthorizeLevel]
        public async Task<IActionResult> Create(FuncProgramCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var funcProgram = new FuncProgram();
                funcProgram.InjectFrom(dto);
                funcProgram.UserLog.CreateTime = DateTime.Now;
                funcProgram.UserLog.CreateUserId = UserId;

                uow.FuncProgramRepo.Add(funcProgram);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.FuncPrograms}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.FuncProgram)]
        [ViewBagFuncGroupId]
        [ViewBagProgramId]
        [ViewBagAuthorizeLevel]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return BadRequest();
            FuncProgram funcProgram = await uow.FuncProgramRepo.FindAsync(id);
            if (funcProgram == null) return NotFound();
            if (funcProgram.AuthorizeLevel > UserAuthorizeLevel)
                return RedirectToAction("NoAuthorization", "Dashbroad");
            var dto = new FuncProgramCreateOrEdit();
            dto.InjectFrom(funcProgram);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.FuncProgram)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagFuncGroupId]
        [ViewBagProgramId]
        [ViewBagAuthorizeLevel]
        public async Task<IActionResult> Edit(FuncProgramCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                FuncProgram funcProgram = await uow.FuncProgramRepo.FindAsync(dto.FuncProgramId, isANT: false);
                if (funcProgram == null) return NotFound();
                if (funcProgram.AuthorizeLevel > UserAuthorizeLevel)
                    return RedirectToAction("NoAuthorization", "Dashbroad");
                funcProgram.InjectFrom(dto);
                funcProgram.UserLog.UpdateTime = DateTime.Now;
                funcProgram.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.FuncPrograms}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.FuncProgram)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return BadRequest();
            FuncProgram funcProgram = await uow.FuncProgramRepo.FindAsync(id);
            if (funcProgram == null) return NotFound();
            if (funcProgram.AuthorizeLevel > UserAuthorizeLevel)
                return RedirectToAction("NoAuthorization", "Dashbroad");
            var dto = new FuncProgramDetailsOrDelete();
            dto.InjectFrom(funcProgram);
            dto.FuncGroupName = funcProgram.FuncGroup.Name;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.FuncProgram)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string FuncProgramId)
        {
            FuncProgram funcProgram = await uow.FuncProgramRepo.FindAsync(FuncProgramId, isANT: false);
            if (funcProgram == null) return NotFound();
            if (funcProgram.AuthorizeLevel > UserAuthorizeLevel)
                return RedirectToAction("NoAuthorization", "Dashbroad");

            var userGroupFuncPrograms = funcProgram.UserGroupFuncPrograms.ToList();
            foreach (var userGroupFuncProgram in userGroupFuncPrograms)
                uow.UserGroupFuncProgramRepo.Delete(userGroupFuncProgram);

            uow.FuncProgramRepo.Delete(funcProgram);
            TempData["success-msg"] = $"{StrText.FuncPrograms}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
