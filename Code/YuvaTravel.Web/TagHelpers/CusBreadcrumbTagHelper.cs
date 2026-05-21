using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// 後台麵包屑。
    ///   4 層: &lt;cus-breadcrumb category="..." resource="..." current="..." /&gt;
    ///   3 層: &lt;cus-breadcrumb category="..." current="..." /&gt;
    ///   2 層: &lt;cus-breadcrumb current="..." /&gt;  (例: 錯誤頁)
    /// 進階用法:
    ///   - 跨 controller 的 resource 連結: resource-controller="Guides"
    ///   - 額外路由參數: resource-route-guideId="@Model.GuideId"
    ///   - 完全自訂連結: resource-href="@Url.Action(...)"
    /// </summary>
    [HtmlTargetElement("cus-breadcrumb")]
    public class CusBreadcrumbTagHelper : TagHelper
    {
        private const string ResourceRouteValuesPrefix = "resource-route-";

        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly HtmlEncoder _encoder;

        public CusBreadcrumbTagHelper(IUrlHelperFactory urlHelperFactory, HtmlEncoder encoder)
        {
            _urlHelperFactory = urlHelperFactory;
            _encoder = encoder;
        }

        [HtmlAttributeName("category")]
        public string? Category { get; set; }

        [HtmlAttributeName("resource")]
        public string? Resource { get; set; }

        [HtmlAttributeName("resource-href")]
        public string? ResourceHref { get; set; }

        [HtmlAttributeName("resource-controller")]
        public string? ResourceController { get; set; }

        [HtmlAttributeName("resource-route-values", DictionaryAttributePrefix = ResourceRouteValuesPrefix)]
        public IDictionary<string, string> ResourceRouteValues { get; set; } = new Dictionary<string, string>();

        [HtmlAttributeName("hide-dashboard")]
        public bool HideDashboard { get; set; }

        [HtmlAttributeName("current")]
        public string Current { get; set; } = default!;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"col-sm-6\"><ol class=\"breadcrumb float-sm-right\">");

            if (!HideDashboard)
            {
                sb.Append("<li class=\"breadcrumb-item\"><a href=\"/admin\">")
                  .Append(_encoder.Encode(StrText.Dashboard))
                  .Append("</a></li>");
            }

            if (!string.IsNullOrWhiteSpace(Category))
            {
                sb.Append("<li class=\"breadcrumb-item\">")
                  .Append(_encoder.Encode(Category))
                  .Append("</li>");
            }

            if (!string.IsNullOrWhiteSpace(Resource))
            {
                var href = ResourceHref ?? BuildIndexHref();
                sb.Append("<li class=\"breadcrumb-item\"><a href=\"")
                  .Append(_encoder.Encode(href))
                  .Append("\">")
                  .Append(_encoder.Encode(Resource))
                  .Append("</a></li>");
            }

            sb.Append("<li class=\"breadcrumb-item active\">")
              .Append(_encoder.Encode(Current ?? string.Empty))
              .Append("</li>");

            sb.Append("</ol></div>");

            output.TagName = null;
            output.Content.SetHtmlContent(sb.ToString());
        }

        private string BuildIndexHref()
        {
            var url = _urlHelperFactory.GetUrlHelper(new ActionContext(
                ViewContext.HttpContext,
                ViewContext.RouteData,
                ViewContext.ActionDescriptor));

            var controller = ResourceController
                ?? ViewContext.RouteData.Values["controller"]?.ToString();

            var routeValues = new RouteValueDictionary { ["area"] = "Admin" };
            foreach (var kv in ResourceRouteValues)
            {
                routeValues[kv.Key] = kv.Value;
            }

            return url.Action("Index", controller, routeValues) ?? string.Empty;
        }
    }
}
