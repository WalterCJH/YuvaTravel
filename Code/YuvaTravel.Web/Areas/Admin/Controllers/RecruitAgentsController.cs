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
using YuvaTravel.Data.Dtos.RecruitAgents;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class RecruitAgentsController : BaseController
    {
        public RecruitAgentsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.RecruitAgent)]
        public IActionResult Index(RecruitAgentFilter filter)
        {
            if (ModelState.IsValid)
                ViewBag.ListData = uow.RecruitAgentRepo.Search(filter).ToPagedList(filter.PageNo, 20);
            else
                ViewBag.ListData = new List<RecruitAgent>().ToPagedList(filter.PageNo, 20);
            return View(filter);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Details, ProgramId.RecruitAgent)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return BadRequest();
            RecruitAgent recruitAgent = await uow.RecruitAgentRepo.FindAsync(id);
            if (recruitAgent == null) return NotFound();
            return View(recruitAgent);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.RecruitAgent)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return BadRequest();
            RecruitAgent recruitAgent = await uow.RecruitAgentRepo.FindAsync(id);
            if (recruitAgent == null) return NotFound();
            var dto = new RecruitAgentEdit();
            dto.InjectFrom(recruitAgent);
            dto.RecruitAgentId = recruitAgent.RecruitAgentId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.RecruitAgent)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RecruitAgentEdit dto)
        {
            if (ModelState.IsValid)
            {
                RecruitAgent recruitAgent = await uow.RecruitAgentRepo.FindAsync(dto.RecruitAgentId, isANT: false);
                if (recruitAgent == null) return NotFound();
                recruitAgent.InjectFrom(dto);
                recruitAgent.UserLog.UpdateTime = DateTime.Now;
                recruitAgent.UserLog.UpdateUserId = UserId;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.RecruitAgents}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.RecruitAgent)]
        public async Task<IActionResult> Contacts(Guid? id)
        {
            if (id == null) return BadRequest();
            RecruitAgent recruitAgent = await uow.RecruitAgentRepo.FindAsync(id);
            if (recruitAgent == null) return NotFound();
            var dto = new RecruitAgentContact();
            dto.InjectFrom(recruitAgent);
            dto.RecruitAgentId = recruitAgent.RecruitAgentId;
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.RecruitAgent)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contacts(RecruitAgentContact dto)
        {
            if (ModelState.IsValid)
            {
                RecruitAgent recruitAgent = await uow.RecruitAgentRepo.FindAsync(dto.RecruitAgentId, isANT: false);
                if (recruitAgent == null) return NotFound();
                recruitAgent.IsContact = dto.IsContact;
                if (recruitAgent.ContactTime == null)
                    recruitAgent.ContactTime = DateTime.Now;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.RecruitAgents}{StrText.EditSuccess}";
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.RecruitAgent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            RecruitAgent recruitAgent = await uow.RecruitAgentRepo.FindAsync(id);
            if (recruitAgent == null) return NotFound();
            return View(recruitAgent);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Delete, ProgramId.RecruitAgent)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            RecruitAgent recruitAgent = await uow.RecruitAgentRepo.FindAsync(id, isANT: false);
            if (recruitAgent == null) return NotFound();
            uow.RecruitAgentRepo.Delete(recruitAgent);
            TempData["success-msg"] = $"{StrText.RecruitAgents}{StrText.DeleteSuccess}";
            await uow.CommitAsync();
            return RedirectToAction("Index");
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Export, ProgramId.RecruitAgent)]
        public IActionResult Export()
        {
            string exportName = "招募代理";

            var workbook = new XLWorkbook();
            var wsSheet = workbook.Worksheets.Add(exportName);
            int row = 1;
            int col = 1;

            var columnName = new List<string> {
                "暱稱", "電話", "TelegramID", "LineID", "其他", "聯繫時間", "新增時間", "新增使用者", "更新時間", "更新使用者"
            };

            foreach (var item in columnName)
            {
                wsSheet.Cell(row, col).Value = item;
                wsSheet.Cell(row, col).Style.Alignment.WrapText = true;
                col++;
            }

            var datas = uow.RecruitAgentRepo.All().OrderByDescending(c => c.UserLog.CreateTime).ToList();

            foreach (var data in datas)
            {
                row++;
                col = 1;

                wsSheet.Cell(row, col++).Value = data.Nick;
                wsSheet.Cell(row, col++).Value = data.Phone;
                wsSheet.Cell(row, col++).Value = data.TelegramID;
                wsSheet.Cell(row, col++).Value = data.LineID;
                wsSheet.Cell(row, col++).Value = data.Other;
                wsSheet.Cell(row, col++).Value = data.ContactTime?.ToString("yyyy/MM/dd HH:mm:ss");
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
