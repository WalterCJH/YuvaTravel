using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Base;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Web.Extensions;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Controllers
{
    public abstract class BaseController : Controller, IExceptionFilter
    {
        private ILogger _logger;
        // DI 取得的 ILogger;沿用實際衍生 controller 型別當分類名(等同舊 GetCurrentClassLogger)。
        // 只在 action/filter/exception 階段使用,此時 HttpContext 一定存在。
        protected ILogger logger => _logger ??= HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

        protected IUnitOfWork uow;

        protected string Host { get; set; } = "";
        protected WebConfig WebConfig { get; set; }
        protected string GoRegister { get; set; } = "";
        protected string UserIP { get; set; } = "";

        /// <summary>
        /// 文章頁要覆寫 meta/OG 用。Action 內設定,OnActionExecuted 讀取組 MetaDto。
        /// 用同請求的 controller 實體屬性傳遞,而非 TempData
        /// (TempData 請求結束會用 BsonTempDataSerializer 序列化,無法序列化 EF 實體)。
        /// </summary>
        protected Article? MetaArticle { get; set; }

        protected BaseController(IUnitOfWork unitOfWork)
        {
            uow = unitOfWork;
            WebConfig = uow.WebConfigRepo.Find() ?? new WebConfig();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var request = filterContext.HttpContext.Request;
            var userAgent = request.Headers["User-Agent"].ToString();
            var absoluteUri = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            foreach (var header in request.Headers)
            {
                sb.AppendLine($"{header.Key}:{header.Value}");
            }
            logger.LogDebug("{Detail}", sb.ToString());

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
                IsOnActionExecuting = true
            };

            #region 取得使用者真實IP
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
            #endregion

            #region UrlReferrerCode
            var rcValues = request.Query["rc"].ToArray();
            if (rcValues.Length > 0)
            {
                urlReferrer.ReferrerCode = rcValues[0];
                var urlReferrerCode = uow.UrlReferrerCodeRepo.All()
                    .FirstOrDefault(p => p.Code == urlReferrer.ReferrerCode);
                urlReferrer.UrlReferrerCodeId = urlReferrerCode?.UrlReferrerCodeId;
            }
            #endregion

            HttpContext.Session.Set(StrSession.BaseController_UrlReferrerId, urlReferrer.UrlReferrerId);
            uow.UrlReferrerRepo.Add(urlReferrer);
            uow.Commit();

            #endregion

            ViewBag.IpAddress = urlReferrer.IpAddress;
            UserIP = urlReferrer.IpAddress;

            #region Host

            ViewBag.Host = Host = $"{request.Scheme}://{request.Host}";

            #endregion

            string promotionCode = StringHelper.GetPromotionCode(request.Path.Value?.ToLower() ?? "");
            GoRegister = StrPath.ClickUrl(Host, $"{WebConfig.GoRegister}{promotionCode}");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var request = filterContext.HttpContext.Request;
            var absoluteUri = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            var absolutePath = request.Path.Value ?? "";
            var queryString = request.QueryString.Value ?? "";

            logger.LogTrace("IP:{UserIP},OnActionExecuted-Start", UserIP);

            var urlReferrerId = HttpContext.Session.Get<Guid?>(StrSession.BaseController_UrlReferrerId);
            if (urlReferrerId.HasValue)
            {
                // 要 tracking 才能改 IsOnActionExecuted
                var dbUrlReferrer = uow.UrlReferrerRepo.Find(urlReferrerId.Value);
                if (dbUrlReferrer != null)
                {
                    dbUrlReferrer.IsOnActionExecuted = true;
                    uow.Commit();
                }
                HttpContext.Session.Remove(StrSession.BaseController_UrlReferrerId);
            }

            #region Meta

            string absoluteUriTrim = absoluteUri.TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(queryString))
                absoluteUriTrim = absoluteUriTrim.Replace(queryString, "");

            var metaDto = new MetaDto();

            if (MetaArticle != null)
            {
                metaDto.Title = MetaArticle.MetaTitle;
                metaDto.Description = MetaArticle.MetaDescription;
                metaDto.OgType = "article";
                metaDto.OgImage = $"{Host}{MetaArticle.ImagePath}";
                metaDto.OgUrl = absoluteUriTrim;
                metaDto.Canonical = absoluteUriTrim;
            }
            else
            {
                var webMeta = uow.WebMetaRepo.All()
                    .FirstOrDefault(p => p.Url.ToUpper() == absolutePath.ToUpper());
                if (webMeta == null)
                {
                    webMeta = new WebMeta();
                    webMeta.MetaDescription = StrText.UserWebName;
                    webMeta.OgType = OgType.website.ToString();
                    metaDto.OgImage = $"{Host}{StrPath.Logo}";
                }
                metaDto.Title = webMeta.MetaTitle;
                metaDto.Description = webMeta.MetaDescription;
                metaDto.OgType = webMeta.OgType;
                if (!string.IsNullOrEmpty(webMeta.MetaImageUrl))
                    metaDto.OgImage = $"{Host}{webMeta.MetaImageUrl}";
                else
                    metaDto.OgImage = $"{Host}{StrPath.Logo}";
                metaDto.OgUrl = absoluteUriTrim;
                if (webMeta.Canonical != null)
                    metaDto.Canonical = webMeta.Canonical;
                else if (webMeta.IsCanonicalAbsoluteUri)
                    metaDto.Canonical = absoluteUriTrim;
            }

            if (metaDto.Title != null && !metaDto.Title.ToLower().Contains(StrText.UserWebName.ToLower()))
                metaDto.Title += $" - {StrText.UserWebName}";

            if (metaDto.Description != null && !metaDto.Description.ToLower().Contains(StrText.UserWebName.ToLower()))
                metaDto.Description += $" - {StrText.UserWebName}";

            ViewBag.MetaDto = metaDto;
            ViewBag.AbsoluteUri = absoluteUriTrim;

            #endregion

            var articleCategories = uow.ArticleCategoryRepo.All().ToList();
            var navBarDto = new NavBarDto(Host, absolutePath, queryString, articleCategories, GoRegister);
            ViewBag.NavBarDto = navBarDto;
            ViewBag.FooterDto = articleCategories
                .OrderBy(p => p.DisplaySeq).Take(3)
                .Select(p => new FooterCategoryDto() { Name = p.Name, Url = p.Url }).ToList();
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception.Message == StrText.AccessDenied)
            {
                logger.LogTrace("{Detail}", MessageHelper.LogMessage(context.Exception, "", $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}", StringHelper.GetBrowserInfo(Request)));
            }
            else
            {
                logger.LogError("{Detail}", MessageHelper.LogMessage(context.Exception, "", $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}", StringHelper.GetBrowserInfo(Request)));
                context.Result = RedirectToAction("Error", "Home");
                context.ExceptionHandled = true;
            }
        }
    }
}
