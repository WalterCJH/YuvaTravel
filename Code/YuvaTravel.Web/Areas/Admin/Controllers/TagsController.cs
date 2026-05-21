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
using YuvaTravel.Data.Dtos.Tags;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class TagsController : BaseController
    {
        IUnitOfWork uow;

        public TagsController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            uow = unitOfWork;
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.Tag)]
        public IActionResult Index(TagFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.TagRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<Tag>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.Tag)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            Tag tag = await uow.TagRepo.FindAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Tag)]
        public IActionResult Create()
        {
            var dto = new TagCreateOrEdit { DisplaySeq = uow.TagRepo.GetMaxDisplaySeq() };
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Create, ProgramId.Tag)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                var tag = new Tag { TagId = Guid.NewGuid() };
                tag.InjectFrom(dto);
                tag.UserLog.CreateTime = DateTime.Now;
                tag.UserLog.CreateUserId = UserId;

                uow.TagRepo.Add(tag);
                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Tags}{StrText.CreateSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Tag)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            Tag tag = await uow.TagRepo.FindAsync(id);   // GET 顯示用,ANT
            if (tag == null) return NotFound();
            var dto = new TagCreateOrEdit();
            dto.InjectFrom(tag);
            dto.TagId = tag.TagId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.Tag)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TagCreateOrEdit dto)
        {
            if (ModelState.IsValid)
            {
                // ⚠️ Edit POST 要 tracking
                Tag tag = await uow.TagRepo.FindAsync(dto.TagId, isANT: false);
                if (tag == null) return NotFound();
                tag.InjectFrom(dto);
                tag.UserLog.UpdateTime = DateTime.Now;
                tag.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.Tags}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Tag)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Tag tag = await uow.TagRepo.FindAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.Tag)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Delete 用 tracking 較安全
            Tag tag = await uow.TagRepo.FindAsync(id, isANT: false);
            if (tag == null) return NotFound();
            uow.TagRepo.Delete(tag);
            TempData["success-msg"] = $"{StrText.Tags}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 標籤搜尋
        /// </summary>
        public JsonResult QuerySelectTags(string searchTerm)
        {
            var tags = new List<Tag>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                tags = uow.TagRepo.IncludeAll().Where(p => p.Name.Contains(searchTerm)).ToList();
            }

            var modifiedDate = tags.Select(p => new
            {
                id = p.TagId,
                text = p.Name
            });

            return Json(modifiedDate);
        }

        /// <summary>
        /// 透過標籤Id，傳回select2Tag資料
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> QuerySelectTagsForTagId([FromBody] List<Guid> tagIds)
        {
            if (tagIds == null || tagIds.Count == 0)
                return Json(Array.Empty<object>());

            // 一次 query 比逐筆 FirstOrDefault 快
            var tags = await uow.TagRepo.All()
                .Where(p => tagIds.Contains(p.TagId))
                .Select(p => new { id = p.TagId, text = p.Name })
                .ToListAsync();

            return Json(tags);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Export, ProgramId.IpBlocking)]
        public IActionResult Export()
        {
            string exportName = "標籤";

            var workbook = new XLWorkbook();
            var wsSheet = workbook.Worksheets.Add(exportName);
            int row = 1;
            int col = 1;

            var columnName = new List<string> {
                "順序", "名稱", "敘述", "文章使用數", "文章標題", "新增時間", "新增使用者", "更新時間", "更新使用者"
            };

            foreach (var item in columnName)
            {
                wsSheet.Cell(row, col).Value = item;
                wsSheet.Cell(row, col).Style.Alignment.WrapText = true;
                col++;
            }

            // Export 用 IncludeAll (含 ArticleTags) 顯示文章使用數與標題
            var datas = uow.TagRepo.IncludeAll().OrderByDescending(c => c.DisplaySeq).ToList();

            foreach (var data in datas)
            {
                row++;
                col = 1;

                wsSheet.Cell(row, col++).Value = data.DisplaySeq;
                wsSheet.Cell(row, col++).Value = data.Name;
                wsSheet.Cell(row, col++).Value = data.Description;
                wsSheet.Cell(row, col++).Value = data.ArticleTags.Count();

                string tmp = "";
                foreach (var item in data.ArticleTags.OrderBy(p => p.DisplaySeq))
                {
                    tmp += $"{item.Article.Title}\r\n";
                }
                var t = "";
                if (tmp.Length > 0)
                {
                    t = tmp.Remove(tmp.Length - 2, 2);
                }
                wsSheet.Cell(row, col++).Value = t;

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
