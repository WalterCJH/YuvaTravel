using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Guides;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.ExcelDto;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Infrastructure.Storage;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Web.Helpers;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class GuidesController : BaseController
    {
        IWebHostEnvironment _env;
        IImageUploadService _imgUpload;

        public GuidesController(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
            IImageUploadService imageUploadService)
            : base(unitOfWork)
        {
            _env = env;
            _imgUpload = imageUploadService;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.Guide)]
        public IActionResult Index(GuideFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.GuideRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<Guide>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.Guide)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            var guide = await uow.GuideRepo.FindAsync(id);
            if (guide == null) return NotFound();
            return View(guide);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Guide)]
        public async Task<IActionResult> Create()
        {
            var dto = new GuideCreateOrEdit
            {
                DisplaySeq = uow.GuideRepo.GetMaxDisplaySeq()
            };
            dto.SettingRegions(await uow.RegionRepo.All().ToListAsync());
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Guide)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GuideCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var guide = new Guide { GuideId = Guid.NewGuid() };
                guide.InjectFrom(dto);
                guide.Code = guide.Code?.Trim();
                guide.UserLog.CreateTime = DateTime.Now;
                guide.UserLog.CreateUserId = UserId;

                await SaveImagesAsync(guide, dto);

                // 必須先 Add Guide 進 context,後續 GuideTag / GuideRegion 才能掛上 FK
                uow.GuideRepo.Add(guide);

                if (dto.IsFeatured)
                    await uow.GuideRepo.ClearOtherFeatured(guide.GuideId, UserId);

                uow.TagRepo.CreateTag(dto.TagIds, UserId);
                uow.Commit();   // 新 Tag + Guide 一起 flush,後續 junction 才能 FK 對得上

                uow.GuideTagRepo.UpdateGuideTagFromGuide(guide.GuideId, dto.TagIds);
                await uow.GuideRegionRepo.UpdateGuideRegions(dto.Regions, guide.GuideId, UserId);

                await uow.CommitAsync();

                TempData["success-msg"] = $"{StrText.Guides}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Guide)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            var guide = await uow.GuideRepo.FindAsync(id);
            if (guide == null) return NotFound();

            var dto = new GuideCreateOrEdit();
            dto.InjectFrom(guide);
            dto.GuideId = guide.GuideId;
            dto.TagIds = uow.GuideTagRepo.QueryTagFromGuide(guide.GuideId);
            var activeRegionIds = guide.GuideRegions?.Select(r => r.RegionId).ToList() ?? new List<Guid>();
            dto.SettingRegions(await uow.RegionRepo.All().ToListAsync(), activeRegionIds);
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Guide)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GuideCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                var guide = await uow.GuideRepo.FindAsync(dto.GuideId, isANT: false);
                if (guide == null) return NotFound();

                // 記下「修改前」的本月精選狀態
                bool wasFeatured = guide.IsFeatured;

                guide.InjectFrom(dto);
                guide.Code = guide.Code?.Trim();
                guide.UserLog.UpdateTime = DateTime.Now;
                guide.UserLog.UpdateUserId = UserId;

                await SaveImagesAsync(guide, dto);

                if (!wasFeatured && dto.IsFeatured)
                    await uow.GuideRepo.ClearOtherFeatured(guide.GuideId, UserId);

                uow.TagRepo.CreateTag(dto.TagIds, UserId);
                await uow.CommitAsync();   // 新 Tag flush 拿 ID

                uow.GuideTagRepo.UpdateGuideTagFromGuide(guide.GuideId, dto.TagIds);
                await uow.GuideRegionRepo.UpdateGuideRegions(dto.Regions, guide.GuideId, UserId);

                await uow.CommitAsync();

                TempData["success-msg"] = $"{StrText.Guides}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        private async Task SaveImagesAsync(Guide guide, GuideCreateOrEdit dto)
        {
            var folder = new[] { Folder.Guides, guide.GuideId.ToString() };

            if (!string.IsNullOrWhiteSpace(dto.PortraitFileBase64))
            {
                await _imgUpload.DeleteAsync(guide.PortraitUrl);
                var baseName = Path.GetFileNameWithoutExtension(dto.PortraitFileName ?? "portrait");
                guide.PortraitUrl = await _imgUpload.UploadAsync(dto.PortraitFileBase64, ImageSize.GuidePortraitWidth, folder, baseName);
            }

            if (!string.IsNullOrWhiteSpace(dto.CoverFileBase64))
            {
                await _imgUpload.DeleteAsync(guide.CoverUrl);
                var baseName = Path.GetFileNameWithoutExtension(dto.CoverFileName ?? "cover");
                guide.CoverUrl = await _imgUpload.UploadAsync(dto.CoverFileBase64, ImageSize.GuideCoverWidth, folder, baseName);
            }
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Guide)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var guide = await uow.GuideRepo.FindAsync(id);
            if (guide == null) return NotFound();
            return View(guide);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Guide)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var guide = await uow.GuideRepo.FindAsync(id, isANT: false);
            if (guide == null) return NotFound();
            uow.GuideRepo.Delete(guide);
            await uow.CommitAsync();
            TempData["success-msg"] = $"{StrText.Guides}{StrText.DeleteSuccess}";
            return RedirectToAction("Index");
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.Guide)]
        public IActionResult Import() => View(new GuideImport());

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.Guide)]
        public IActionResult ImportEx()
        {
            string fileName = $"{StrText.Guides}匯入範例檔";

            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(StrText.Guides);

            var headers = new List<string>
            {
                "啟用","本月精選","代碼","中文名","英文名",
                "在地年數","語言","短簡介","評分","評論數","服務形式","起價",
                "肖像圖URL","封面圖URL","順序"
            };
            for (int i = 0; i < headers.Count; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
            }

            ws.Cell(2, 1).Value = "Y";
            ws.Cell(2, 2).Value = "Y";
            ws.Cell(2, 3).Value = "misaki";
            ws.Cell(2, 4).Value = "美咲";
            ws.Cell(2, 5).Value = "Misaki Tanaka";
            ws.Cell(2, 6).Value = 7;
            ws.Cell(2, 7).Value = "JP · EN";
            ws.Cell(2, 8).Value = "在東山長大,最熟下雨天的茶屋路線。";
            ws.Cell(2, 9).Value = 4.9;
            ws.Cell(2, 10).Value = 38;
            ws.Cell(2, 11).Value = "半日 / 全日";
            ws.Cell(2, 12).Value = 4800;
            ws.Cell(2, 13).Value = "";
            ws.Cell(2, 14).Value = "";
            ws.Cell(2, 15).Value = 100;
            ws.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return File(ms.ToArray(), "application/excel", $"{fileName}.xlsx");
            }
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.Guide)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(GuideImport dto)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(_env.WebRootPath, Folder.TempFile, dto.File.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using (var stream = System.IO.File.Create(path)) { await dto.File.CopyToAsync(stream); }

                var mappings = new Dictionary<string, string>
                {
                    { nameof(GuideImportExcelDto.IsActive), "啟用" },
                    { nameof(GuideImportExcelDto.IsFeatured), "本月精選" },
                    { nameof(GuideImportExcelDto.Code), "代碼" },
                    { nameof(GuideImportExcelDto.Name), "中文名" },
                    { nameof(GuideImportExcelDto.NameEn), "英文名" },
                    { nameof(GuideImportExcelDto.YearsOfExperience), "在地年數" },
                    { nameof(GuideImportExcelDto.Languages), "語言" },
                    { nameof(GuideImportExcelDto.Bio), "短簡介" },
                    { nameof(GuideImportExcelDto.Rating), "評分" },
                    { nameof(GuideImportExcelDto.ReviewCount), "評論數" },
                    { nameof(GuideImportExcelDto.ServiceFormat), "服務形式" },
                    { nameof(GuideImportExcelDto.StartingPrice), "起價" },
                    { nameof(GuideImportExcelDto.PortraitUrl), "肖像圖URL" },
                    { nameof(GuideImportExcelDto.CoverUrl), "封面圖URL" },
                    { nameof(GuideImportExcelDto.DisplaySeq), "順序" }
                };
                var rows = ExcelImportHelper.ReadExcelWithMappings<GuideImportExcelDto>(path, mappings);

                int maxDisplaySeq = uow.GuideRepo.GetMaxDisplaySeq();

                foreach (var row in rows)
                {
                    Guide guide = null;
                    if (!string.IsNullOrWhiteSpace(row.Code))
                        guide = await uow.GuideRepo.FindCodeAsync(row.Code, isANT: false);
                    if (guide == null)
                        guide = uow.GuideRepo.All(isANT: false).FirstOrDefault(p => p.Name == row.Name);

                    if (guide == null)
                    {
                        guide = new Guide
                        {
                            GuideId = Guid.NewGuid(),
                            DisplaySeq = row.DisplaySeq > 0 ? row.DisplaySeq : maxDisplaySeq++
                        };
                        guide.UserLog.CreateTime = DateTime.Now;
                        guide.UserLog.CreateUserId = UserId;
                        uow.GuideRepo.Add(guide);
                    }
                    else
                    {
                        guide.UserLog.UpdateTime = DateTime.Now;
                        guide.UserLog.UpdateUserId = UserId;
                    }

                    guide.IsActive = row.IsActive;
                    guide.IsFeatured = row.IsFeatured;
                    guide.Code = row.Code?.Trim();
                    guide.Name = row.Name;
                    guide.NameEn = row.NameEn;
                    guide.YearsOfExperience = row.YearsOfExperience;
                    guide.Languages = row.Languages;
                    guide.Bio = row.Bio;
                    guide.Rating = row.Rating;
                    guide.ReviewCount = row.ReviewCount;
                    guide.ServiceFormat = row.ServiceFormat;
                    guide.StartingPrice = row.StartingPrice;
                    guide.PortraitUrl = row.PortraitUrl;
                    guide.CoverUrl = row.CoverUrl;
                    if (row.DisplaySeq > 0) guide.DisplaySeq = row.DisplaySeq;
                }

                await uow.CommitAsync();

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                TempData["success-msg"] = $"{StrText.Guides}{StrText.UploadSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }
    }
}
