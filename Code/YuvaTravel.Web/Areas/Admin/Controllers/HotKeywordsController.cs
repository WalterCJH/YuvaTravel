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
using YuvaTravel.Data.Dtos.HotKeywords;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.ExcelDto;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Web.Helpers;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HotKeywordsController : BaseController
    {
        IWebHostEnvironment _env;

        public HotKeywordsController(IUnitOfWork unitOfWork, IWebHostEnvironment env) : base(unitOfWork)
        {
            _env = env;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.HotKeyword)]
        public IActionResult Index(HotKeywordFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.HotKeywordRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<HotKeyword>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.HotKeyword)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            HotKeyword hotKeyword = await uow.HotKeywordRepo.FindAsync(id);
            if (hotKeyword == null) return NotFound();
            return View(hotKeyword);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.HotKeyword)]
        public IActionResult Create()
        {
            var dto = new HotKeywordCreateOrEdit { DisplaySeq = uow.HotKeywordRepo.GetMaxDisplaySeq() };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.HotKeyword)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotKeywordCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var hotKeyword = new HotKeyword { HotKeywordId = Guid.NewGuid() };
                hotKeyword.InjectFrom(dto);
                hotKeyword.UserLog.CreateTime = DateTime.Now;
                hotKeyword.UserLog.CreateUserId = UserId;

                uow.HotKeywordRepo.Add(hotKeyword);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.HotKeywords}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.HotKeyword)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            HotKeyword hotKeyword = await uow.HotKeywordRepo.FindAsync(id);
            if (hotKeyword == null) return NotFound();
            var dto = new HotKeywordCreateOrEdit();
            dto.InjectFrom(hotKeyword);
            dto.HotKeywordId = hotKeyword.HotKeywordId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.HotKeyword)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HotKeywordCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                HotKeyword hotKeyword = await uow.HotKeywordRepo.FindAsync(dto.HotKeywordId, isANT: false);
                if (hotKeyword == null) return NotFound();
                hotKeyword.InjectFrom(dto);
                hotKeyword.UserLog.UpdateTime = DateTime.Now;
                hotKeyword.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.HotKeywords}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.HotKeyword)]
        public async Task<IActionResult> Delete(Guid id)
        {
            HotKeyword hotKeyword = await uow.HotKeywordRepo.FindAsync(id);
            if (hotKeyword == null) return NotFound();
            return View(hotKeyword);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.HotKeyword)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            HotKeyword hotKeyword = await uow.HotKeywordRepo.FindAsync(id, isANT: false);
            if (hotKeyword == null) return NotFound();
            uow.HotKeywordRepo.Delete(hotKeyword);
            TempData["success-msg"] = $"{StrText.HotKeywords}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.HotKeyword)]
        public IActionResult Import() => View();

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.HotKeyword)]
        public IActionResult ImportEx()
        {
            string fileName = $"{StrText.HotKeywords}匯入範例檔";

            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(StrText.HotKeywords);

            var headers = new List<string> { "啟用", "名稱" };
            for (int i = 0; i < headers.Count; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
            }

            ws.Cell(2, 1).Value = "Y";
            ws.Cell(2, 2).Value = "範例關鍵字";

            ws.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return File(ms.ToArray(), "application/excel", $"{fileName}.xlsx");
            }
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Import, ProgramId.HotKeyword)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(HotKeywordImport dto)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(_env.WebRootPath, Folder.TempFile, dto.File.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using (var stream = System.IO.File.Create(path)) { await dto.File.CopyToAsync(stream); }

                var mappings = new Dictionary<string, string>
                {
                    { nameof(HotKeywordImportExcelDto.IsActive), "啟用" },
                    { nameof(HotKeywordImportExcelDto.Name), "名稱" }
                };
                var excelRows = ExcelImportHelper.ReadExcelWithMappings<HotKeywordImportExcelDto>(path, mappings);

                int maxDisplaySeq = uow.HotKeywordRepo.GetMaxDisplaySeq();

                foreach (var row in excelRows)
                {
                    // Import 是 upsert,要 tracking
                    var hotKeyword = uow.HotKeywordRepo.All(isANT: false).FirstOrDefault(p => p.Name == row.Name);
                    if (hotKeyword == null)
                    {
                        hotKeyword = new HotKeyword
                        {
                            HotKeywordId = Guid.NewGuid(),
                            DisplaySeq = maxDisplaySeq++
                        };
                        hotKeyword.UserLog.CreateTime = DateTime.Now;
                        hotKeyword.UserLog.CreateUserId = UserId;
                        uow.HotKeywordRepo.Add(hotKeyword);
                    }
                    else
                    {
                        hotKeyword.UserLog.UpdateTime = DateTime.Now;
                        hotKeyword.UserLog.UpdateUserId = UserId;
                    }

                    hotKeyword.IsActive = row.IsActive;
                    hotKeyword.Name = row.Name;
                }

                await uow.CommitAsync();

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                TempData["success-msg"] = $"{StrText.HotKeywords}{StrText.UploadSuccess}";
                return RedirectToAction("Index");
            }

            return View(dto);
        }
    }
}
