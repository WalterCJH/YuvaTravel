using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// 日期挑選器 (yyyy/MM/dd)。等同舊 EditorTemplates/Date.cshtml。
    /// 用法: &lt;cus-date asp-for="BeginTime" /&gt;
    /// </summary>
    [HtmlTargetElement("cus-date", Attributes = ForAttributeName)]
    public class CusDateTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        private readonly IHtmlGenerator _generator;

        public CusDateTagHelper(IHtmlGenerator generator) => _generator = generator;

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; } = default!;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "input-group");

            var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var value = For.Model is DateTime dt ? dt.ToString("yyyy/MM/dd") : null;
            var htmlAttrs = new Dictionary<string, object> { ["class"] = "form-control js-datepicker" };

            var textbox = _generator.GenerateTextBox(
                ViewContext, For.ModelExplorer, fullName, value, format: null, htmlAttributes: htmlAttrs);

            output.Content.AppendHtml(
                "<div class=\"input-group-append\"><div class=\"input-group-text\"><i class=\"fa fa-calendar\"></i></div></div>");
            output.Content.AppendHtml(textbox);
        }
    }
}
