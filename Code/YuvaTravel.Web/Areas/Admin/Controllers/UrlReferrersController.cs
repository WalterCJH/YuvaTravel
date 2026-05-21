using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.UrlReferrers;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class UrlReferrersController : BaseController
    {
        public UrlReferrersController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.UrlReferrer)]
        public IActionResult Index()
        {
            return View(new UrlReferrerExport());
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Index, ProgramId.UrlReferrer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(UrlReferrerExport dto)
        {
            if (ModelState.IsValid)
            {
                string exportName = "網頁來源";

                var workbook = new XLWorkbook();
                var wsSheet = workbook.Worksheets.Add(exportName);
                int row = 1;
                int col = 1;

                var columnName = new List<string> {
                    "使用者IP位置", "CouldFlareIP位置", "來源網址", "目的網址", "點擊時間", "代碼",
                    "電腦", "手機、平板", "使用者資訊", "使用者系統", "系統名稱", "瀏覽器名稱", "瀏覽器版本",
                    "ActionExecuting", "ActionExecuted"
                };

                foreach (var item in columnName)
                {
                    wsSheet.Cell(row, col).Value = item;
                    wsSheet.Cell(row, col).Style.Alignment.WrapText = true;
                    col++;
                }

                var dateRangeDto = StringHelper.GetDateTimeRangeData(dto.RangeTime);

                var datas = uow.UrlReferrerRepo.All()
                    .Where(p => p.CreateTime >= dateRangeDto.StartTime &&
                                p.CreateTime <= dateRangeDto.EndTime &&
                                p.IpAddress != StrIpAddress.Company1 &&
                                p.IpAddress != StrIpAddress.Company2 &&
                                p.IpAddress != StrIpAddress.Company3)
                    .OrderBy(c => c.CreateTime).ToList();

                foreach (var data in datas)
                {
                    row++;
                    col = 1;

                    wsSheet.Cell(row, col++).Value = data.IpAddress;
                    wsSheet.Cell(row, col++).Value = data.CloudFlareIpAddress;
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
            return View(dto);
        }
    }
}
