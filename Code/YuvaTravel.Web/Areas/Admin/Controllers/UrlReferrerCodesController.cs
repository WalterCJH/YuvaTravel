using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.UrlReferrerCodes;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.ExcelDto;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Web.Helpers;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class UrlReferrerCodesController : BaseController
    {
        IWebHostEnvironment _env;

        public UrlReferrerCodesController(IUnitOfWork unitOfWork, IWebHostEnvironment env) : base(unitOfWork)
        {
            _env = env;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.UrlReferrerCode)]
        public IActionResult Index(UrlReferrerCodeFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.UrlReferrerCodeRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<UrlReferrerCode>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.UrlReferrerCode)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            UrlReferrerCode urlReferrerCode = await uow.UrlReferrerCodeRepo.FindAsync(id);
            if (urlReferrerCode == null) return NotFound();
            return View(urlReferrerCode);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.UrlReferrerCode)]
        public IActionResult Create() => View();

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.UrlReferrerCode)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UrlReferrerCodeCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var urlReferrerCode = new UrlReferrerCode { UrlReferrerCodeId = Guid.NewGuid() };
                urlReferrerCode.InjectFrom(dto);
                urlReferrerCode.UserLog.CreateTime = DateTime.Now;
                urlReferrerCode.UserLog.CreateUserId = UserId;

                uow.UrlReferrerCodeRepo.Add(urlReferrerCode);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.UrlReferrerCodes}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.UrlReferrerCode)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            UrlReferrerCode urlReferrerCode = await uow.UrlReferrerCodeRepo.FindAsync(id);
            if (urlReferrerCode == null) return NotFound();
            var dto = new UrlReferrerCodeCreateOrEdit();
            dto.InjectFrom(urlReferrerCode);
            dto.UrlReferrerCodeId = urlReferrerCode.UrlReferrerCodeId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.UrlReferrerCode)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UrlReferrerCodeCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                UrlReferrerCode urlReferrerCode = await uow.UrlReferrerCodeRepo.FindAsync(dto.UrlReferrerCodeId, isANT: false);
                if (urlReferrerCode == null) return NotFound();
                urlReferrerCode.InjectFrom(dto);
                urlReferrerCode.UserLog.UpdateTime = DateTime.Now;
                urlReferrerCode.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.UrlReferrerCodes}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.UrlReferrerCode)]
        public async Task<IActionResult> Delete(Guid id)
        {
            UrlReferrerCode urlReferrerCode = await uow.UrlReferrerCodeRepo.FindAsync(id);
            if (urlReferrerCode == null) return NotFound();
            if (urlReferrerCode.UrlReferrers.Count() > 0)
                return RedirectToAction("Index");
            return View(urlReferrerCode);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.UrlReferrerCode)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid UrlReferrerCodeId)
        {
            UrlReferrerCode urlReferrerCode = await uow.UrlReferrerCodeRepo.FindAsync(UrlReferrerCodeId, isANT: false);
            if (urlReferrerCode == null) return NotFound();
            if (urlReferrerCode.UrlReferrers.Count() > 0)
                return RedirectToAction("Index");
            uow.UrlReferrerCodeRepo.Delete(urlReferrerCode);
            TempData["success-msg"] = $"{StrText.UrlReferrerCodes}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.UrlReferrerCode)]
        public IActionResult Import() => View();

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.UrlReferrerCode)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(UrlReferrerCodeImport dto)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(_env.WebRootPath, Folder.TempFile, dto.File.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using (var stream = System.IO.File.Create(path)) { await dto.File.CopyToAsync(stream); }

                var mappings = new Dictionary<string, string>
                {
                    { nameof(UrlReferrerCodeImportExcelDto.Code), "代碼" },
                    { nameof(UrlReferrerCodeImportExcelDto.Name), "名稱" },
                    { nameof(UrlReferrerCodeImportExcelDto.Description), "描述" }
                };
                var excelRows = ExcelImportHelper.ReadExcelWithMappings<UrlReferrerCodeImportExcelDto>(path, mappings);

                foreach (var row in excelRows)
                {
                    var urlReferrerCode = uow.UrlReferrerCodeRepo.All(isANT: false).FirstOrDefault(p => p.Code == row.Code);
                    if (urlReferrerCode == null)
                    {
                        urlReferrerCode = new UrlReferrerCode { UrlReferrerCodeId = Guid.NewGuid() };
                        urlReferrerCode.UserLog.CreateTime = DateTime.Now;
                        urlReferrerCode.UserLog.CreateUserId = UserId;
                        uow.UrlReferrerCodeRepo.Add(urlReferrerCode);
                    }
                    else
                    {
                        urlReferrerCode.UserLog.UpdateTime = DateTime.Now;
                        urlReferrerCode.UserLog.UpdateUserId = UserId;
                    }

                    urlReferrerCode.Code = row.Code;
                    urlReferrerCode.Name = row.Name;
                    urlReferrerCode.Description = row.Description;
                }

                await uow.CommitAsync();

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                TempData["success-msg"] = $"{StrText.UrlReferrerCodes}{StrText.UploadSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }


        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Export, ProgramId.UrlReferrerCode)]
        public IActionResult Export()
        {
            string exportName = "網頁來源代碼";

            var workbook = new XLWorkbook();
            var wsSheet = workbook.Worksheets.Add(exportName);
            int row = 1;
            int col = 1;

            var columnName = new List<string> {
                "使用者IP位置", "來源網址", "目的網址", "點擊時間", "代碼",
                "電腦", "手機、平板", "使用者資訊", "使用者系統", "系統名稱", "瀏覽器名稱", "瀏覽器版本",
                "ActionExecuting", "ActionExecuted"
            };

            foreach (var item in columnName)
            {
                wsSheet.Cell(row, col).Value = item;
                wsSheet.Cell(row, col).Style.Alignment.WrapText = true;
                col++;
            }

            var datas = uow.UrlReferrerRepo.All().Where(p => p.ReferrerCode != null &&
                !p.SourceUrl.StartsWith("http://www.bing.com") &&
                !p.SourceUrl.StartsWith("https://r.search.yahoo.com") &&
                !p.SourceUrl.StartsWith("https://tw.search.yahoo.com") &&
                !p.SourceUrl.StartsWith("https://www.google.com") &&
                !p.SourceUrl.StartsWith(StrText.WebSiteUrl) &&
                !p.SourceUrl.StartsWith("https://localhost") &&
                p.IpAddress != StrIpAddress.Company1 &&
                p.IpAddress != StrIpAddress.Company2
            ).OrderByDescending(c => c.CreateTime).ToList();

            foreach (var data in datas)
            {
                row++;
                col = 1;

                wsSheet.Cell(row, col++).Value = data.IpAddress;
                wsSheet.Cell(row, col++).Value = data.SourceUrl;
                wsSheet.Cell(row, col++).Value = data.DestinationUrl;
                wsSheet.Cell(row, col++).Value = data.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                wsSheet.Cell(row, col++).Value = data.ReferrerCode;

                if (data.IsMobile)
                {
                    wsSheet.Cell(row, col++).Value = "";
                    wsSheet.Cell(row, col++).Value = "Y";
                }
                else
                {
                    wsSheet.Cell(row, col++).Value = "Y";
                    wsSheet.Cell(row, col++).Value = "";
                }
                wsSheet.Cell(row, col++).Value = data.UserAgent;
                wsSheet.Cell(row, col++).Value = data.Platform;
                wsSheet.Cell(row, col++).Value = data.OS;
                wsSheet.Cell(row, col++).Value = data.BrowserType;
                wsSheet.Cell(row, col++).Value = data.BrowserVersion;
                wsSheet.Cell(row, col++).Value = (data.IsOnActionExecuting) ? "Y" : "";
                wsSheet.Cell(row, col++).Value = (data.IsOnActionExecuted) ? "Y" : "";
            }

            wsSheet.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return File(ms.ToArray(), "application/excel", $"{exportName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
        }
    }
}
