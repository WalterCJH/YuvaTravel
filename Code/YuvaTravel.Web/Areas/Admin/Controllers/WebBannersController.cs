using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.WebBanners;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Storage;
using YuvaTravel.Web.ActionFilters.ViewBag;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class WebBannersController : BaseController
    {
        IUnitOfWork uow;
        IWebHostEnvironment _env;
        IImageUploadService _imgUpload;

        public WebBannersController(IUnitOfWork unitOfWork, IWebHostEnvironment env, IImageUploadService imageUploadService) : base(unitOfWork)
        {
            uow = unitOfWork;
            _env = env;
            _imgUpload = imageUploadService;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.WebBanner)]
        public IActionResult Index(WebBannerFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.WebBannerRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<WebBanner>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.WebBanner)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            WebBanner webBanner = await uow.WebBannerRepo.FindAsync(id);   // 顯示用,ANT
            if (webBanner == null) return NotFound();
            return View(webBanner);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.WebBanner)]
        [ViewBagOgType]
        public IActionResult Create()
        {
            var dto = new WebBannerCreateOrEdit
            {
                DisplaySeq = uow.WebBannerRepo.GetMaxDisplaySeq(),
                BeginTime = DateTime.Now,
                EndTime = DateTime.Now.AddMonths(1),
                ImageClickUrl = "#"
            };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.WebBanner)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagOgType]
        public async Task<IActionResult> Create(WebBannerCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var webBanner = new WebBanner { WebBannerId = Guid.NewGuid() };
                webBanner.InjectFrom(dto);
                webBanner.UserLog.CreateTime = DateTime.Now;
                webBanner.UserLog.CreateUserId = UserId;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var folder = new[] { Folder.WebBanners };
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageFile.FileName ?? "banner");
                    webBanner.ImageUrl       = await _imgUpload.UploadAsync(dto.ImageFile, ImageSize.BannerWidth,       folder, baseName);
                    webBanner.ImageMobileUrl = await _imgUpload.UploadAsync(dto.ImageFile, ImageSize.BannerMobileWidth, folder, baseName + "-m");
                }

                uow.WebBannerRepo.Add(webBanner);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.WebBanners}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.WebBanner)]
        [ViewBagOgType]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            WebBanner webBanner = await uow.WebBannerRepo.FindAsync(id);   // GET 顯示用,ANT
            if (webBanner == null) return NotFound();
            var dto = new WebBannerCreateOrEdit();
            dto.InjectFrom(webBanner);
            dto.WebBannerId = webBanner.WebBannerId;
            dto.ImageUrl_ = webBanner.ImageUrl;
            dto.ImageMobileUrl_ = webBanner.ImageMobileUrl;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.WebBanner)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ViewBagOgType]
        public async Task<IActionResult> Edit(WebBannerCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                WebBanner webBanner = await uow.WebBannerRepo.FindAsync(dto.WebBannerId, isANT: false);
                if (webBanner == null) return NotFound();

                webBanner.InjectFrom(dto);
                webBanner.UserLog.UpdateTime = DateTime.Now;
                webBanner.UserLog.UpdateUserId = UserId;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    await _imgUpload.DeleteAsync(webBanner.ImageUrl);
                    await _imgUpload.DeleteAsync(webBanner.ImageMobileUrl);

                    var folder = new[] { Folder.WebBanners };
                    var baseName = Path.GetFileNameWithoutExtension(dto.ImageFile.FileName ?? "banner");
                    webBanner.ImageUrl       = await _imgUpload.UploadAsync(dto.ImageFile, ImageSize.BannerWidth,       folder, baseName);
                    webBanner.ImageMobileUrl = await _imgUpload.UploadAsync(dto.ImageFile, ImageSize.BannerMobileWidth, folder, baseName + "-m");
                }

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.WebBanners}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.WebBanner)]
        public async Task<IActionResult> Delete(Guid id)
        {
            WebBanner webBanner = await uow.WebBannerRepo.FindAsync(id);
            if (webBanner == null) return NotFound();
            return View(webBanner);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.WebBanner)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid WebBannerId)
        {
            // Delete 用 tracking 較安全
            WebBanner webBanner = await uow.WebBannerRepo.FindAsync(WebBannerId, isANT: false);
            if (webBanner == null) return NotFound();
            // 清掉 R2 / 本機檔案 (舊版漏掉,跟 WebMeta 同樣的 bug)
            await _imgUpload.DeleteAsync(webBanner.ImageUrl);
            await _imgUpload.DeleteAsync(webBanner.ImageMobileUrl);
            uow.WebBannerRepo.Delete(webBanner);
            TempData["success-msg"] = $"{StrText.WebBanners}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
