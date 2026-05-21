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
using YuvaTravel.Data.Dtos.IpOpenings;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.ExcelDto;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Web.Helpers;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class IpOpeningsController : BaseController
    {
        IWebHostEnvironment _env;

        public IpOpeningsController(IUnitOfWork unitOfWork, IWebHostEnvironment env) : base(unitOfWork)
        {
            _env = env;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.IpOpening)]
        public IActionResult Index(IpOpeningFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.IpOpeningRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<IpOpening>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.IpOpening)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            IpOpening ipOpening = await uow.IpOpeningRepo.FindAsync(id);
            if (ipOpening == null) return NotFound();
            return View(ipOpening);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.IpOpening)]
        public IActionResult Create()
        {
            return View(new IpOpeningCreateOrEdit { IsActive = true });
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.IpOpening)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IpOpeningCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var ipOpening = new IpOpening { IpOpeningId = Guid.NewGuid() };
                ipOpening.InjectFrom(dto);
                ipOpening.UserLog.CreateTime = DateTime.Now;
                ipOpening.UserLog.CreateUserId = UserId;

                uow.IpOpeningRepo.Add(ipOpening);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.IpOpenings}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.IpOpening)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            IpOpening ipOpening = await uow.IpOpeningRepo.FindAsync(id);
            if (ipOpening == null) return NotFound();
            var dto = new IpOpeningCreateOrEdit();
            dto.InjectFrom(ipOpening);
            dto.IpOpeningId = ipOpening.IpOpeningId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.IpOpening)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IpOpeningCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                IpOpening ipOpening = await uow.IpOpeningRepo.FindAsync(dto.IpOpeningId, isANT: false);
                if (ipOpening == null) return NotFound();
                ipOpening.InjectFrom(dto);
                ipOpening.UserLog.UpdateTime = DateTime.Now;
                ipOpening.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.IpOpenings}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.IpOpening)]
        public async Task<IActionResult> Delete(Guid id)
        {
            IpOpening ipOpening = await uow.IpOpeningRepo.FindAsync(id);
            if (ipOpening == null) return NotFound();
            return View(ipOpening);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.IpOpening)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            IpOpening ipOpening = await uow.IpOpeningRepo.FindAsync(id, isANT: false);
            if (ipOpening == null) return NotFound();
            uow.IpOpeningRepo.Delete(ipOpening);
            TempData["success-msg"] = $"{StrText.IpOpenings}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.IpOpening)]
        public IActionResult Import() => View();

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.IpOpening)]
        public IActionResult ImportEx()
        {
            string fileName = $"{StrText.IpOpenings}匯入範例檔";

            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(StrText.IpOpenings);

            var headers = new List<string> { "啟用", "IP", "備註" };
            for (int i = 0; i < headers.Count; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
            }

            ws.Cell(2, 1).Value = "Y";
            ws.Cell(2, 2).Value = "192.168.1.1";
            ws.Cell(2, 3).Value = "範例備註";

            ws.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return File(ms.ToArray(), "application/excel", $"{fileName}.xlsx");
            }
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.IpOpening)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IpOpeningImport dto)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(_env.WebRootPath, Folder.TempFile, dto.File.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using (var stream = System.IO.File.Create(path)) { await dto.File.CopyToAsync(stream); }

                var mappings = new Dictionary<string, string>
                {
                    { nameof(IpOpeningImportExcelDto.IsActive), "啟用" },
                    { nameof(IpOpeningImportExcelDto.IpAddress), "IP" },
                    { nameof(IpOpeningImportExcelDto.Memo), "備註" }
                };
                var excelRows = ExcelImportHelper.ReadExcelWithMappings<IpOpeningImportExcelDto>(path, mappings);

                foreach (var row in excelRows)
                {
                    if (string.IsNullOrWhiteSpace(row.IpAddress)) continue;

                    var ipOpening = uow.IpOpeningRepo.All(isANT: false).FirstOrDefault(p => p.IpAddress == row.IpAddress || p.IpAddress.Contains(row.IpAddress));
                    if (ipOpening == null)
                    {
                        ipOpening = new IpOpening { IpOpeningId = Guid.NewGuid() };
                        ipOpening.UserLog.CreateTime = DateTime.Now;
                        ipOpening.UserLog.CreateUserId = UserId;
                        uow.IpOpeningRepo.Add(ipOpening);
                    }
                    else
                    {
                        ipOpening.UserLog.UpdateTime = DateTime.Now;
                        ipOpening.UserLog.UpdateUserId = UserId;
                    }

                    ipOpening.IsActive = row.IsActive;
                    ipOpening.IpAddress = row.IpAddress;
                    if (uow.IpOpeningRepo.IsIncompleteIP(ipOpening.IpAddress))
                    {
                        ipOpening.IsIncompleteIP = true;
                        ipOpening.IpAddress = uow.IpOpeningRepo.IncompleteIPAddDot(ipOpening.IpAddress);
                    }
                    ipOpening.Memo = row.Memo;
                }

                await uow.CommitAsync();

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                TempData["success-msg"] = $"{StrText.IpOpenings}{StrText.UploadSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Export, ProgramId.IpOpening)]
        public IActionResult Export()
        {
            string exportName = "IP白名單";

            var workbook = new XLWorkbook();
            var wsSheet = workbook.Worksheets.Add(exportName);
            int row = 1;
            int col = 1;

            var columnName = new List<string> { "啟用", "IP", "備註", "新增時間", "新增使用者", "更新時間", "更新使用者" };

            foreach (var item in columnName)
            {
                wsSheet.Cell(row, col).Value = item;
                wsSheet.Cell(row, col).Style.Alignment.WrapText = true;
                col++;
            }

            var datas = uow.IpOpeningRepo.All().OrderByDescending(c => c.UserLog.CreateTime).ToList();

            foreach (var data in datas)
            {
                row++;
                col = 1;

                wsSheet.Cell(row, col++).Value = (data.IsActive) ? "Y" : "";
                wsSheet.Cell(row, col++).Value = data.IpAddress;
                wsSheet.Cell(row, col++).Value = data.Memo;
                wsSheet.Cell(row, col++).Value = data.UserLog.CreateTime?.ToString("yyyy/MM/dd HH:mm:ss");
                wsSheet.Cell(row, col++).Value = data.UserLog.CreateUserId;
                wsSheet.Cell(row, col++).Value = data.UserLog.UpdateTime?.ToString("yyyy/MM/dd HH:mm:ss");
                wsSheet.Cell(row, col++).Value = data.UserLog.UpdateUserId;
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
