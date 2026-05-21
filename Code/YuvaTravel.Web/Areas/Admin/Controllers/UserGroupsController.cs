using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.UserGroups;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class UserGroupsController : BaseController
    {
        public UserGroupsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.UserGroup)]
        [ViewBagAuthorizeLevel]
        public IActionResult Index(UserGroupFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.UserGroupRepo.Search(filter, UserAuthorizeLevel).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<UserGroup>().ToPagedList(filter.PageNo, 20);

            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.UserGroup)]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return BadRequest();
            UserGroup userGroup = await uow.UserGroupRepo.FindAsync(id);
            if (userGroup == null) return NotFound();
            var dto = new UserGroupCreateOrEdit(uow.FuncGroupRepo.IncludeAll().ToList(), userGroup, UserId, UserAuthorizeLevel);
            dto.InjectFrom(userGroup);

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.UserGroup)]
        [ViewBagAuthorizeLevel]
        public IActionResult Create()
        {
            var dto = new UserGroupCreateOrEdit(uow.FuncGroupRepo.IncludeAll().ToList(), null, UserId, UserAuthorizeLevel);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.UserGroup)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagAuthorizeLevel]
        public async Task<IActionResult> Create(UserGroupCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var userGroup = new UserGroup();
                userGroup.InjectFrom(dto);
                userGroup.UserLog.CreateTime = DateTime.Now;
                userGroup.UserLog.CreateUserId = UserId;

                // 必須先 Add UserGroup 進 context,再做 junction (UserGroupFuncProgram) 更新,
                // 否則之後 commit 時 FK_UserGroupFuncProgram_UserGroup 會炸 (參考 Articles 的舊雷)
                uow.UserGroupRepo.Add(userGroup);

                // 權限更新
                await uow.UserGroupFuncProgramRepo.UpdateUserGroupFuncProgram(dto.FuncGroups, userGroup, UserId);

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.UserGroups}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.UserGroup)]
        [ViewBagSalutation]
        [ViewBagAuthorizeLevel]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return BadRequest();
            UserGroup userGroup = await uow.UserGroupRepo.FindAsync(id);
            if (userGroup == null) return NotFound();
            if (userGroup.AuthorizeLevel > UserAuthorizeLevel)
                return RedirectToAction("NoAuthorization", "Dashbroad");

            var dto = new UserGroupCreateOrEdit(uow.FuncGroupRepo.IncludeAll().ToList(), userGroup, UserId, UserAuthorizeLevel);
            dto.InjectFrom(userGroup);

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.UserGroup)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagSalutation]
        [ViewBagAuthorizeLevel]
        public async Task<IActionResult> Edit(UserGroupCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                UserGroup userGroup = await uow.UserGroupRepo.FindAsync(dto.UserGroupId, isANT: false);
                if (userGroup == null) return NotFound();
                if (userGroup.AuthorizeLevel > UserAuthorizeLevel)
                    return RedirectToAction("NoAuthorization", "Dashbroad");
                userGroup.InjectFrom(dto);
                userGroup.UserLog.UpdateTime = DateTime.Now;
                userGroup.UserLog.UpdateUserId = UserId;

                // 權限更新
                await uow.UserGroupFuncProgramRepo.UpdateUserGroupFuncProgram(dto.FuncGroups, userGroup, UserId);

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.UserGroups}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.UserGroup)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return BadRequest();
            UserGroup userGroup = await uow.UserGroupRepo.FindAsync(id);
            if (userGroup == null) return NotFound();
            if (userGroup.AuthorizeLevel > UserAuthorizeLevel)
                return RedirectToAction("NoAuthorization", "Dashbroad");
            var dto = new UserGroupCreateOrEdit(uow.FuncGroupRepo.IncludeAll().ToList(), userGroup, UserId, UserAuthorizeLevel);
            dto.InjectFrom(userGroup);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.UserGroup)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string UserGroupId)
        {
            UserGroup userGroup = await uow.UserGroupRepo.FindAsync(UserGroupId, isANT: false);
            if (userGroup == null) return NotFound();
            if (userGroup.AuthorizeLevel > UserAuthorizeLevel)
                return RedirectToAction("NoAuthorization", "Dashbroad");

            var userGroupFuncPrograms = userGroup.UserGroupFuncPrograms.ToList();
            foreach (var userGroupFuncProgram in userGroupFuncPrograms)
                uow.UserGroupFuncProgramRepo.Delete(userGroupFuncProgram);

            uow.UserGroupRepo.Delete(userGroup);
            TempData["success-msg"] = $"{StrText.UserGroups}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
