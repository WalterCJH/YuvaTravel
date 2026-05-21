using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Users;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.User)]
        [ViewBagUserGroupId]
        public IActionResult Index(UserFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.UserRepo.Search(filter, UserGroupId).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<User>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.User)]
        public async Task<IActionResult> Details(Guid id)
        {
            User user = await uow.UserRepo.FindGuidAsync(id);
            if (user == null) return NotFound();
            var dto = new UserDetailsOrDelete();
            dto.InjectFrom(user);
            dto.UserGroupName = user.UserGroup.Name;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.User)]
        [ViewBagUserGroupId]
        [ViewBagSalutation]
        public IActionResult Create()
        {
            return View(new UserCreateOrEdit());
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.User)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagUserGroupId]
        [ViewBagSalutation]
        public async Task<IActionResult> Create(UserCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var user = new User();
                user.InjectFrom(dto);
                user.UserGuid = Guid.NewGuid();
                if (!string.IsNullOrWhiteSpace(dto._Password)) user.Password = uow.UserRepo.HashPassword(dto._Password);
                string tmpUserId;
                if (RegularHelper.IsEng(user.LastName + user.FirstName))
                    tmpUserId = $"{user.FirstName}{user.LastName}";
                else
                    tmpUserId = $"{user.LastName}{user.FirstName}";
                user.UserId = uow.UserRepo.GetNotRepeatUserId(tmpUserId);
                user.UserLog.CreateTime = DateTime.Now;
                user.UserLog.CreateUserId = UserId;

                uow.UserRepo.Add(user);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Users}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.User)]
        [ViewBagUserGroupId]
        [ViewBagSalutation]
        public async Task<IActionResult> Edit(Guid id)
        {
            User user = await uow.UserRepo.FindGuidAsync(id);
            if (user == null) return NotFound();
            var dto = new UserCreateOrEdit();
            dto.InjectFrom(user);
            dto.UserGuid = user.UserGuid;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.User)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagUserGroupId]
        [ViewBagSalutation]
        public async Task<IActionResult> Edit(UserCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                User user = await uow.UserRepo.FindGuidAsync(dto.UserGuid.Value, isANT: false);
                if (user == null) return NotFound();
                user.InjectFrom(dto);
                if (!string.IsNullOrWhiteSpace(dto._Password)) user.Password = uow.UserRepo.HashPassword(dto._Password);
                user.UserLog.UpdateTime = DateTime.Now;
                user.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Users}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.User)]
        public async Task<IActionResult> Delete(Guid id)
        {
            User user = await uow.UserRepo.FindGuidAsync(id);
            if (user == null) return NotFound();
            var dto = new UserDetailsOrDelete();
            dto.InjectFrom(user);
            dto.UserGroupName = user.UserGroup.Name;
            dto.UserGuid = user.UserGuid;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.User)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            User user = await uow.UserRepo.FindGuidAsync(id, isANT: false);
            if (user == null) return NotFound();
            uow.UserRepo.Delete(user);
            TempData["success-msg"] = $"{StrText.Users}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
