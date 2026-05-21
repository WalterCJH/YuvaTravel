using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.WebConfigs;
using YuvaTravel.Data.Entities;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    public class WebConfigsController : BaseController
    {
        public WebConfigsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.WebConfig)]
        public async Task<IActionResult> Edit()
        {
            var dto = new WebConfigEdit();
            WebConfig webConfig = await uow.WebConfigRepo.FindAsync();
            if (webConfig != null)
            {
                dto.WebConfigId = webConfig.WebConfigId;
                dto.GoRegister = webConfig.GoRegister;
            }
            return View(dto);
        }

        [Area("Admin")]
        [AuthorizeAdmin(ActionMode.Edit, ProgramId.WebConfig)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WebConfigEdit dto)
        {
            if (ModelState.IsValid)
            {
                // Edit POST 要 tracking
                WebConfig webConfig = await uow.WebConfigRepo.FindAsync(isANT: false);
                if (webConfig == null)
                {
                    webConfig = new WebConfig
                    {
                        WebConfigId = Guid.NewGuid()
                    };
                    webConfig.UserLog.CreateTime = DateTime.Now;
                    webConfig.UserLog.CreateUserId = UserId;
                    uow.WebConfigRepo.Add(webConfig);
                }
                else
                {
                    webConfig.UserLog.UpdateTime = DateTime.Now;
                    webConfig.UserLog.UpdateUserId = UserId;
                }
                webConfig.GoRegister = dto.GoRegister;

                await uow.CommitAsync();
                TempData["success-msg"] = $"{StrText.WebConfigs}{StrText.EditSuccess}";
                return RedirectToAction("Edit");
            }
            return View(dto);
        }
    }
}
