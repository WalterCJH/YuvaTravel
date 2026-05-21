using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Regions;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Storage;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class RegionsController : BaseController
    {
        IUnitOfWork uow;
        IImageUploadService _imgUpload;

        public RegionsController(IUnitOfWork unitOfWork, IImageUploadService imageUploadService) : base(unitOfWork)
        {
            uow = unitOfWork;
            _imgUpload = imageUploadService;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.Region)]
        public IActionResult Index(RegionFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.RegionRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<Region>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.Region)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            var entity = await uow.RegionRepo.FindAsync(id);   // 顯示用,ANT
            if (entity == null) return NotFound();
            return View(entity);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Region)]
        public IActionResult Create()
        {
            var dto = new RegionCreateOrEdit { DisplaySeq = uow.RegionRepo.GetMaxDisplaySeq() };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Region)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegionCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var entity = new Region { RegionId = Guid.NewGuid() };
                entity.InjectFrom(dto);
                entity.Code = entity.Code?.Trim();
                entity.UserLog.CreateTime = DateTime.Now;
                entity.UserLog.CreateUserId = UserId;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var folder = new[] { Folder.Regions };
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageFile.FileName ?? entity.Code ?? "region");
                    entity.ImageUrl = await _imgUpload.UploadAsync(dto.ImageFile, ImageSize.RegionWidth, folder, baseName);
                }

                uow.RegionRepo.Add(entity);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Regions}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Region)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            var entity = await uow.RegionRepo.FindAsync(id);   // GET 顯示用,ANT
            if (entity == null) return NotFound();

            var dto = new RegionCreateOrEdit();
            dto.InjectFrom(entity);
            dto.RegionId = entity.RegionId;
            dto.ImageUrl_ = entity.ImageUrl;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Region)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegionCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                var entity = await uow.RegionRepo.FindAsync(dto.RegionId, isANT: false);
                if (entity == null) return NotFound();

                entity.InjectFrom(dto);
                entity.Code = entity.Code?.Trim();
                entity.UserLog.UpdateTime = DateTime.Now;
                entity.UserLog.UpdateUserId = UserId;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    await _imgUpload.DeleteAsync(entity.ImageUrl);
                    var folder = new[] { Folder.Regions };
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageFile.FileName ?? entity.Code ?? "region");
                    entity.ImageUrl = await _imgUpload.UploadAsync(dto.ImageFile, ImageSize.RegionWidth, folder, baseName);
                }

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Regions}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Region)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await uow.RegionRepo.FindAsync(id);   // 顯示確認頁,ANT
            if (entity == null) return NotFound();
            return View(entity);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Region)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Delete 用 tracking 較安全
            var entity = await uow.RegionRepo.FindAsync(id, isANT: false);
            if (entity == null) return NotFound();
            await _imgUpload.DeleteAsync(entity.ImageUrl);
            uow.RegionRepo.Delete(entity);
            await uow.CommitAsync();
            TempData["success-msg"] = $"{StrText.Regions}{StrText.DeleteSuccess}";
            return RedirectToAction("Index");
        }
    }
}
