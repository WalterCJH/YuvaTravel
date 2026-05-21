using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// OG Type 下拉選單 (從 ViewBag.OgType 取得來源)。等同舊 EditorTemplates/OgType.cshtml。
    /// 用法: &lt;cus-og-type asp-for="OgType" /&gt;
    /// </summary>
    [HtmlTargetElement("cus-og-type", Attributes = ForAttributeName)]
    public class CusOgTypeTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; } = default!;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;

            var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            output.Attributes.SetAttribute("name", fullName);
            if (!output.Attributes.ContainsName("id"))
            {
                output.Attributes.SetAttribute("id", fullName);
            }
            if (!output.Attributes.ContainsName("class"))
            {
                output.Attributes.SetAttribute("class", "form-control");
            }

            var selectedValue = For.Model?.ToString();
            var items = ViewContext.ViewData["OgType"] as IEnumerable<KeyValuePair<string, string>>;

            var sb = new StringBuilder();
            sb.Append("<option value=\"\">(請選擇)</option>");
            if (items != null)
            {
                foreach (var item in items)
                {
                    var selected = item.Value == selectedValue ? " selected=\"selected\"" : "";
                    sb.Append($"<option value=\"{WebUtility.HtmlEncode(item.Value)}\"{selected}>{WebUtility.HtmlEncode(item.Key)}</option>");
                }
            }
            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
