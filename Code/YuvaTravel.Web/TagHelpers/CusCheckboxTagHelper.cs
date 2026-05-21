using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// 渲染 AdminLTE iCheck 樣式 boolean checkbox：div.icheck-success.d-inline 包 checkbox + hidden + label。
    /// 用法：
    ///   &lt;cus-checkbox asp-for="IsActive" /&gt;             顯示 [Display(Name=...)] 對應 label
    ///   &lt;cus-checkbox asp-for="IsActive" hide-label="true" /&gt; 用 &amp;nbsp; 佔位、不顯示 label 文字
    /// </summary>
    [HtmlTargetElement("cus-checkbox", Attributes = ForAttributeName)]
    public class CusCheckboxTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string HideLabelAttributeName = "hide-label";

        private readonly IHtmlGenerator _generator;

        public CusCheckboxTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; } = default!;

        [HtmlAttributeName(HideLabelAttributeName)]
        public bool HideLabel { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "icheck-success d-inline");

            var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var isChecked = For.Model is bool b && b;

            // checkbox + hidden（hidden 確保 unchecked 時也送 false）
            var checkbox = _generator.GenerateCheckBox(
                ViewContext,
                For.ModelExplorer,
                fullName,
                isChecked,
                htmlAttributes: null);

            var hidden = _generator.GenerateHiddenForCheckbox(
                ViewContext,
                For.ModelExplorer,
                fullName);

            output.Content.AppendHtml(checkbox);
            output.Content.AppendHtml(hidden);

            // label
            if (HideLabel || string.IsNullOrEmpty(For.Metadata.DisplayName))
            {
                output.Content.AppendHtml("<label>&nbsp;</label>");
            }
            else
            {
                var label = _generator.GenerateLabel(
                    ViewContext,
                    For.ModelExplorer,
                    fullName,
                    labelText: null,
                    htmlAttributes: null);
                output.Content.AppendHtml(label);
            }
        }
    }
}
