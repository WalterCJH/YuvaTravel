using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Base;
using YuvaTravel.Web.Extensions;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Filter
{
    public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter
    {
        private ProgramId[] Prgs { get; set; }
        private ActionMode? ActionMode_ { get; set; }
        private bool IsUnauthorized { get; set; }

        public AuthorizeAdminAttribute()
        {
            Prgs = Array.Empty<ProgramId>();
        }

        public AuthorizeAdminAttribute(ActionMode actionMode, params ProgramId[] programIds)
        {
            Prgs = programIds;
            ActionMode_ = actionMode;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var userData = session.Get<UserData>(StrSession.AdminUserData);

            if (userData == null)
            {
                var returnUrl = context.HttpContext.Request.Path;
                context.Result = new RedirectToActionResult("Login", "Account",
                    new { area = "Admin", returnUrl });
                return;
            }

            if (Prgs.Length == 0) return;

            session.Set(StrSession.AdminProgramIds, Prgs);

            if (ActionMode_ != null)
            {
                var uow = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                foreach (var prg in Prgs)
                {
                    var userGroupFuncProgram = uow.UserGroupFuncProgramRepo.All()
                        .FirstOrDefault(p => p.UserGroupId == userData.UserGroupId && p.FuncProgramId == prg.ToString());

                    bool authorized = ActionMode_ switch
                    {
                        ActionMode.Index => userGroupFuncProgram != null,
                        ActionMode.Create => userGroupFuncProgram?.IsCreate == true,
                        ActionMode.Edit => userGroupFuncProgram?.IsEdit == true,
                        ActionMode.Delete => userGroupFuncProgram?.IsDelete == true,
                        ActionMode.Details => userGroupFuncProgram?.IsDetails == true,
                        ActionMode.Import => userGroupFuncProgram?.IsDetails == true,
                        ActionMode.Export => userGroupFuncProgram?.IsDetails == true,
                        _ => false
                    };

                    if (authorized) return;
                }
            }

            context.Result = new RedirectToActionResult("NoAuthorization", "Dashbroad",
                new { area = "Admin" });
        }
    }
}
