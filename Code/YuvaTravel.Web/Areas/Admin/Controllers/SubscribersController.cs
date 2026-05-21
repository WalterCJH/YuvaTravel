using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Subscribers;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.Areas.Admin.Filter;
// 新版 UoW alias,避開與舊 IUnitOfWork 撞名
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class SubscribersController : BaseController
    {
        IUnitOfWork uow;

        public SubscribersController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            uow = unitOfWork;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.Subscriber)]
        public IActionResult Index(SubscriberFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.SubscriberRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<Subscriber>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.Subscriber)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            var entity = await uow.SubscriberRepo.FindAsync(id);   // 顯示用,ANT
            if (entity == null) return NotFound();
            return View(entity);
        }

        /// <summary>切換啟用 / 取消訂閱狀態</summary>
        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Subscriber)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            // ⚠️ 要改屬性 → tracking
            var entity = await uow.SubscriberRepo.FindAsync(id, isANT: false);
            if (entity == null) return NotFound();

            if (entity.IsActive)
            {
                entity.IsActive = false;
                entity.UnsubscribedTime = DateTime.Now;
                TempData["success-msg"] = $"已將 {entity.Email} 設為取消訂閱";
            }
            else
            {
                entity.IsActive = true;
                entity.UnsubscribedTime = null;
                TempData["success-msg"] = $"已重新啟用 {entity.Email} 的訂閱";
            }
            entity.UserLog.UpdateTime = DateTime.Now;
            entity.UserLog.UpdateUserId = UserId;
            await uow.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Subscriber)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await uow.SubscriberRepo.FindAsync(id);   // 顯示確認頁,ANT
            if (entity == null) return NotFound();
            return View(entity);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Subscriber)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Delete 用 tracking 較安全
            var entity = await uow.SubscriberRepo.FindAsync(id, isANT: false);
            if (entity == null) return NotFound();
            uow.SubscriberRepo.Delete(entity);
            await uow.CommitAsync();
            TempData["success-msg"] = $"{StrText.Subscribers}{StrText.DeleteSuccess}";
            return RedirectToAction(nameof(Index));
        }
    }
}
