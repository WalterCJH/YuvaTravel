using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// CKEditor 文字編輯區。等同舊 EditorTemplates/Ckeditor.cshtml:
    ///   @Html.TextArea("", new { @class = "ckeditor" })
    /// 用法:
    ///   &lt;cus-ckeditor asp-for="Content" /&gt;
    /// </summary>
    [HtmlTargetElement("cus-ckeditor", Attributes = ForAttributeName)]
    public class CusCkeditorTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        private readonly IHtmlGenerator _generator;

        public CusCkeditorTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; } = default!;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var htmlAttrs = new Dictionary<string, object> { ["class"] = "ckeditor" };

            var textarea = _generator.GenerateTextArea(
                ViewContext,
                For.ModelExplorer,
                fullName,
                rows: 2,
                columns: 20,
                htmlAttributes: htmlAttrs);

            // 不要外層 <cus-ckeditor> wrapper,只輸出 textarea
            output.TagName = null;
            output.Content.SetHtmlContent(textarea);
        }
    }
}
