using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// 輸出屬性值（等同 Html.DisplayFor），不包覆任何 element。
    /// 用法：
    ///   &lt;cus-display asp-for="Name" /&gt;
    ///       使用 page model 屬性，走完整 DisplayTemplates 機制（含 [UIHint]）
    ///   &lt;cus-display value="@item.IsActive" /&gt;
    ///       直接傳值，依型別格式化：
    ///         bool      → AdminLTE iCheck 樣式 checkbox（同 CheckedNoName.cshtml 行為）
    ///         DateTime  → yyyy/MM/dd HH:mm:ss
    ///         其他      → ToString()（HTML encode 後輸出）
    /// </summary>
    [HtmlTargetElement("cus-display", Attributes = "asp-for")]
    [HtmlTargetElement("cus-display", Attributes = "value")]
    public class CusDisplayTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        public CusDisplayTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        [HtmlAttributeName("asp-for")]
        public ModelExpression? For { get; set; }

        [HtmlAttributeName("value")]
        public object? Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            output.TagMode = TagMode.StartTagAndEndTag;

            if (For != null)
            {
                ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);
                output.Content.SetHtmlContent(_htmlHelper.Display(For.Name));
            }
            else
            {
                output.Content.SetHtmlContent(FormatValue(Value));
            }
        }

        private static IHtmlContent FormatValue(object? value)
        {
            if (value is null)
            {
                return HtmlString.Empty;
            }
            if (value is bool b)
            {
                var checkedAttr = b ? @" checked=""checked""" : string.Empty;
                return new HtmlString(
                    $@"<div class=""icheck-success d-inline""><input type=""checkbox""{checkedAttr} /><label>&nbsp;</label></div>");
            }
            if (value is DateTime dt)
            {
                return new HtmlString(dt.ToString("yyyy/MM/dd HH:mm:ss"));
            }
            return new HtmlString(System.Net.WebUtility.HtmlEncode(value.ToString() ?? string.Empty));
        }
    }
}
