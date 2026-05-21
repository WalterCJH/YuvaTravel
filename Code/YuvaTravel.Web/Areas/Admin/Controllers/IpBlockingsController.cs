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
using YuvaTravel.Data.Dtos.IpBlockings;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.ExcelDto;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Web.Helpers;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class IpBlockingsController : BaseController
    {
        IWebHostEnvironment _env;

        public IpBlockingsController(IUnitOfWork unitOfWork, IWebHostEnvironment env) : base(unitOfWork)
        {
            _env = env;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.IpBlocking)]
        public async Task<IActionResult> Index(IpBlockingFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.IpBlockingRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<IpBlocking>().ToPagedList(filter.PageNo, 20);
            ViewBag.AllDataCount = await uow.IpBlockingRepo.All().CountAsync();
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.IpBlocking)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            IpBlocking ipBlocking = await uow.IpBlockingRepo.FindAsync(id);
            if (ipBlocking == null) return NotFound();
            return View(ipBlocking);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.IpBlocking)]
        public IActionResult Create()
        {
            return View(new IpBlockingCreateOrEdit { IsActive = true });
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.IpBlocking)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IpBlockingCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var ipBlocking = new IpBlocking { IpBlockingId = Guid.NewGuid() };
                ipBlocking.InjectFrom(dto);
                ipBlocking.UserLog.CreateTime = DateTime.Now;
                ipBlocking.UserLog.CreateUserId = UserId;

                uow.IpBlockingRepo.Add(ipBlocking);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.IpBlockings}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.IpBlocking)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            IpBlocking ipBlocking = await uow.IpBlockingRepo.FindAsync(id);
            if (ipBlocking == null) return NotFound();
            var dto = new IpBlockingCreateOrEdit();
            dto.InjectFrom(ipBlocking);
            dto.IpBlockingId = ipBlocking.IpBlockingId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.IpBlocking)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IpBlockingCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                IpBlocking ipBlocking = await uow.IpBlockingRepo.FindAsync(dto.IpBlockingId, isANT: false);
                if (ipBlocking == null) return NotFound();
                ipBlocking.InjectFrom(dto);
                ipBlocking.UserLog.UpdateTime = DateTime.Now;
                ipBlocking.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.IpBlockings}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.IpBlocking)]
        public async Task<IActionResult> Delete(Guid id)
        {
            IpBlocking ipBlocking = await uow.IpBlockingRepo.FindAsync(id);
            if (ipBlocking == null) return NotFound();
            return View(ipBlocking);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.IpBlocking)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            IpBlocking ipBlocking = await uow.IpBlockingRepo.FindAsync(id, isANT: false);
            if (ipBlocking == null) return NotFound();
            uow.IpBlockingRepo.Delete(ipBlocking);
            TempData["success-msg"] = $"{StrText.IpBlockings}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.IpBlocking)]
        public IActionResult Import() => View(new IpBlockingImport());

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.IpBlocking)]
        public IActionResult ImportEx()
        {
            string fileName = $"{StrText.IpBlockings}匯入範例檔";

            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(StrText.IpBlockings);

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
        [AuthorizeAdmin(ActionMode.Import, ProgramId.IpBlocking)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IpBlockingImport dto)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(_env.WebRootPath, Folder.TempFile, dto.File.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using (var stream = System.IO.File.Create(path)) { await dto.File.CopyToAsync(stream); }

                var mappings = new Dictionary<string, string>
                {
                    { nameof(IpBlockingImportExcelDto.IsActive), "啟用" },
                    { nameof(IpBlockingImportExcelDto.IpAddress), "IP" },
                    { nameof(IpBlockingImportExcelDto.Memo), "備註" }
                };
                var excelRows = ExcelImportHelper.ReadExcelWithMappings<IpBlockingImportExcelDto>(path, mappings);

                if (dto.IsReplaceAllData)
                {
                    var dbIpBlockings = await uow.IpBlockingRepo.All(isANT: false).ToListAsync();
                    foreach (var dbIpBlocking in dbIpBlockings)
                        uow.IpBlockingRepo.Delete(dbIpBlocking);

                    foreach (var row in excelRows)
                    {
                        if (string.IsNullOrWhiteSpace(row.IpAddress)) continue;

                        var ipBlocking = new IpBlocking { IpBlockingId = Guid.NewGuid() };
                        ipBlocking.UserLog.CreateTime = DateTime.Now;
                        ipBlocking.UserLog.CreateUserId = UserId;
                        uow.IpBlockingRepo.Add(ipBlocking);

                        ipBlocking.IsActive = row.IsActive;
                        ipBlocking.IpAddress = row.IpAddress;
                        if (uow.IpBlockingRepo.IsIncompleteIP(ipBlocking.IpAddress))
                        {
                            ipBlocking.IsIncompleteIP = true;
                            ipBlocking.IpAddress = uow.IpBlockingRepo.IncompleteIPAddDot(ipBlocking.IpAddress);
                        }
                        ipBlocking.Memo = row.Memo;
                    }
                }
                else
                {
                    foreach (var row in excelRows)
                    {
                        if (string.IsNullOrWhiteSpace(row.IpAddress)) continue;

                        var ipBlocking = uow.IpBlockingRepo.All(isANT: false).FirstOrDefault(p => p.IpAddress == row.IpAddress);
                        if (ipBlocking == null)
                        {
                            ipBlocking = new IpBlocking { IpBlockingId = Guid.NewGuid() };
                            ipBlocking.UserLog.CreateTime = DateTime.Now;
                            ipBlocking.UserLog.CreateUserId = UserId;
                            uow.IpBlockingRepo.Add(ipBlocking);
                        }
                        else
                        {
                            ipBlocking.UserLog.UpdateTime = DateTime.Now;
                            ipBlocking.UserLog.UpdateUserId = UserId;
                        }

                        ipBlocking.IsActive = row.IsActive;
                        ipBlocking.IpAddress = row.IpAddress;
                        if (uow.IpBlockingRepo.IsIncompleteIP(ipBlocking.IpAddress))
                        {
                            ipBlocking.IsIncompleteIP = true;
                            ipBlocking.IpAddress = uow.IpBlockingRepo.IncompleteIPAddDot(ipBlocking.IpAddress);
                        }
                        ipBlocking.Memo = row.Memo;
                    }
                }

                await uow.CommitAsync();

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                TempData["success-msg"] = $"{StrText.IpBlockings}{StrText.UploadSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Export, ProgramId.IpBlocking)]
        public IActionResult Export()
        {
            string exportName = "IP黑名單";

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

            var datas = uow.IpBlockingRepo.All().OrderByDescending(c => c.UserLog.CreateTime).ToList();

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
