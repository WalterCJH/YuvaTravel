using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.WebMetas;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Storage;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class WebMetasController : BaseController
    {
        IUnitOfWork uow;
        IWebHostEnvironment _env;
        IImageUploadService _imgUpload;

        public WebMetasController(IUnitOfWork unitOfWork, IWebHostEnvironment env, IImageUploadService imageUploadService) : base(unitOfWork)
        {
            uow = unitOfWork;
            _env = env;
            _imgUpload = imageUploadService;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.WebMeta)]
        public IActionResult Index(WebMetaFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.WebMetaRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<WebMeta>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.WebMeta)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            WebMeta webMeta = await uow.WebMetaRepo.FindAsync(id);   // 顯示用,ANT
            if (webMeta == null) return NotFound();
            return View(webMeta);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.WebMeta)]
        [ViewBagOgType]
        public IActionResult Create()
        {
            var dto = new WebMetaCreateOrEdit { DisplaySeq = uow.WebMetaRepo.GetMaxDisplaySeq() };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.WebMeta)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagOgType]
        public async Task<IActionResult> Create(WebMetaCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var webMeta = new WebMeta { WebMetaId = Guid.NewGuid() };
                webMeta.InjectFrom(dto);
                webMeta.UserLog.CreateTime = DateTime.Now;
                webMeta.UserLog.CreateUserId = UserId;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var folder = new[] { Folder.WebMetas };
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageFile.FileName ?? "meta");
                    webMeta.MetaImageUrl = await _imgUpload.UploadAsync(dto.ImageFile, ImageSize.WebMetaWidth, folder, baseName);
                }

                uow.WebMetaRepo.Add(webMeta);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.WebMetas}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.WebMeta)]
        [ViewBagOgType]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            WebMeta webMeta = await uow.WebMetaRepo.FindAsync(id);   // GET 顯示用,ANT
            if (webMeta == null) return NotFound();
            var dto = new WebMetaCreateOrEdit();
            dto.InjectFrom(webMeta);
            dto.WebMetaId = webMeta.WebMetaId;
            dto.MetaImageUrl_ = webMeta.MetaImageUrl;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.WebMeta)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagOgType]
        public async Task<IActionResult> Edit(WebMetaCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                WebMeta webMeta = await uow.WebMetaRepo.FindAsync(dto.WebMetaId, isANT: false);
                if (webMeta == null) return NotFound();

                webMeta.InjectFrom(dto);
                webMeta.UserLog.UpdateTime = DateTime.Now;
                webMeta.UserLog.UpdateUserId = UserId;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    await _imgUpload.DeleteAsync(webMeta.MetaImageUrl);
                    var folder = new[] { Folder.WebMetas };
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageFile.FileName ?? "meta");
                    webMeta.MetaImageUrl = await _imgUpload.UploadAsync(dto.ImageFile, ImageSize.WebMetaWidth, folder, baseName);
                }

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.WebMetas}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.WebMeta)]
        public async Task<IActionResult> Delete(Guid id)
        {
            WebMeta webMeta = await uow.WebMetaRepo.FindAsync(id);
            if (webMeta == null) return NotFound();
            return View(webMeta);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.WebMeta)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid webMetaId)
        {
            // Delete 用 tracking 較安全
            WebMeta webMeta = await uow.WebMetaRepo.FindAsync(webMetaId, isANT: false);
            if (webMeta == null) return NotFound();
            await _imgUpload.DeleteAsync(webMeta.MetaImageUrl);   // 順手清掉 R2 / 本機檔案 (舊版漏掉了)
            uow.WebMetaRepo.Delete(webMeta);
            TempData["success-msg"] = $"{StrText.WebMetas}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
