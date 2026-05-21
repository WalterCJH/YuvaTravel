using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Base;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Web.Extensions;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public abstract class BaseController : Controller, IExceptionFilter
    {
        private ILogger _logger;
        // DI 取得的 ILogger;沿用實際衍生 controller 型別當分類名(等同舊 GetCurrentClassLogger)。
        // 只在 action/filter/exception 階段使用,此時 HttpContext 一定存在。
        protected ILogger logger => _logger ??= HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

        protected IUnitOfWork uow;

        protected Guid UserGuid { get; set; }
        protected string UserId { get; set; } = "";
        protected string UserGroupId { get; set; } = "";
        protected string UserName { get; set; } = "";
        protected AuthorizeLevel UserAuthorizeLevel { get; set; }
        protected bool IsSystemLevel { get; set; }
        protected bool IsAdminLevel { get; set; }

        protected SystemConfig SystemConfig { get; set; } = SystemConfig.Production;
        protected string Host { get; set; } = "";
        protected WebConfig WebConfig { get; set; }
        protected string GoRegister { get; set; } = "";

        protected BaseController(IUnitOfWork unitOfWork)
        {
            uow = unitOfWork;
            WebConfig = uow.WebConfigRepo.Find() ?? new WebConfig();

            #if DEBUG
                SystemConfig = SystemConfig.Develop;
            #endif
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var request = filterContext.HttpContext.Request;
            var userAgent = request.Headers["User-Agent"].ToString();
            var absoluteUri = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            #region 網頁來源代碼

            UrlReferrer urlReferrer = new UrlReferrer
            {
                UrlReferrerId = Guid.NewGuid(),
                SourceUrl = request.Headers["Referer"].ToString(),
                DestinationUrl = absoluteUri,
                IsMobile = userAgent.Contains("Mobile") || userAgent.Contains("Android"),
                OS = StringHelper.GetUserPlatform(userAgent),
                UserAgent = userAgent,
                Platform = "",
                BrowserType = "",
                BrowserVersion = "",
                CreateTime = DateTime.Now,
                IsOnActionExecuting = true,
                IsAdmin = true
            };

            var cfIp = request.Headers["cf-connecting-ip"].ToString();
            if (!string.IsNullOrEmpty(cfIp))
            {
                urlReferrer.IpAddress = cfIp.Length > 100 ? cfIp.Substring(0, 100) : cfIp;
                urlReferrer.CloudFlareIpAddress = filterContext.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            }
            else
            {
                urlReferrer.IpAddress = filterContext.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            }

            var rcValues = request.Query["rc"].ToArray();
            if (rcValues.Length > 0)
            {
                urlReferrer.ReferrerCode = rcValues[0];
                var urlReferrerCode = uow.UrlReferrerCodeRepo.All()
                    .FirstOrDefault(p => p.Code == urlReferrer.ReferrerCode);
                urlReferrer.UrlReferrerCodeId = urlReferrerCode?.UrlReferrerCodeId;
            }

            HttpContext.Session.Set(StrSession.AdminBaseController_UrlReferrerId, urlReferrer.UrlReferrerId);
            uow.UrlReferrerRepo.Add(urlReferrer);
            uow.Commit();

            #endregion

            var userData = HttpContext.Session.Get<UserData>(StrSession.AdminUserData);
            if (userData != null)
            {
                UserGuid = userData.UserGuid;
                UserId = userData.UserId;
                UserGroupId = userData.UserGroupId;
                UserName = userData.UserName;
                UserAuthorizeLevel = userData.AuthorizeLevel;
                IsSystemLevel = userData.IsSystemLevel;
                IsAdminLevel = userData.IsAdminLevel;

                var funcGroups = uow.UserRepo.QueryFuncGroupForAuthorize(userData.UserGuid);
                ViewBag.UserName = UserName;
                ViewBag.UserId = UserId;
                ViewBag.AdminNavBarDto = new AdminNavBarDto(request.Path.Value ?? "", funcGroups, userData.UserName);
            }
            else
            {
                ViewBag.AdminNavBarDto = new AdminNavBarDto();
            }

            #region Host

            ViewBag.Host = Host = $"{request.Scheme}://{request.Host}";

            #endregion

            string promotionCode = StringHelper.GetPromotionCode(request.Path.Value?.ToLower() ?? "");
            GoRegister = StrPath.ClickUrl(Host, $"{WebConfig.GoRegister}{promotionCode}");

            #region 權限設定

            var adminProgramIds = HttpContext.Session.Get<ProgramId[]>(StrSession.AdminProgramIds);
            if (adminProgramIds != null)
            {
                foreach (var adminProgramId in adminProgramIds)
                {
                    var userGroupFuncProgram = uow.UserGroupFuncProgramRepo.All()
                        .FirstOrDefault(p => p.UserGroupId == UserGroupId && p.FuncProgramId == adminProgramId.ToString());
                    ViewBag.IsCreate = userGroupFuncProgram?.IsCreate == true;
                    ViewBag.IsEdit = userGroupFuncProgram?.IsEdit == true;
                    ViewBag.IsDelete = userGroupFuncProgram?.IsDelete == true;
                    ViewBag.IsDetails = userGroupFuncProgram?.IsDetails == true;
                    ViewBag.IsImport = userGroupFuncProgram?.IsImport == true;
                    ViewBag.IsExport = userGroupFuncProgram?.IsExport == true;
                }
            }

            #endregion
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var urlReferrerId = HttpContext.Session.Get<Guid?>(StrSession.AdminBaseController_UrlReferrerId);
            if (urlReferrerId.HasValue)
            {
                // 需 tracking 才能改 IsOnActionExecuted 後 commit
                var dbUrlReferrer = uow.UrlReferrerRepo.Find(urlReferrerId.Value);
                if (dbUrlReferrer != null)
                {
                    dbUrlReferrer.IsOnActionExecuted = true;
                    uow.Commit();
                }
                HttpContext.Session.Remove(StrSession.AdminBaseController_UrlReferrerId);
            }
        }

        public void OnException(ExceptionContext context)
        {
            var request = context.HttpContext.Request;
            var absoluteUri = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            logger.LogError("{Detail}", MessageHelper.LogMessage(context.Exception, UserId, absoluteUri, StringHelper.GetBrowserInfo(request)));
            context.Result = RedirectToAction("Error", "Dashbroad", new { Area = "Admin" });
            context.ExceptionHandled = true;
        }
    }
}
