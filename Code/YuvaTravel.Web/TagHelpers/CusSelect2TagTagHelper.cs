using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// 標籤多選 Select2 元件。取代原本 EditorTemplate Select2Tag.cshtml。
    ///
    /// 輸出:
    ///   &lt;select name="TagIds" class="col-12 js-select2-tag" multiple&gt;&lt;/select&gt;
    ///   &lt;div class="TagIds d-none"&gt;{tagId}&lt;/div&gt; …
    ///
    /// 用法:
    ///   &lt;cus-select2-tag asp-for="TagIds" /&gt;
    /// </summary>
    [HtmlTargetElement("cus-select2-tag", Attributes = ForAttributeName)]
    public class CusSelect2TagTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; } = default!;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // 拿掉外層自訂標籤,直接把內容寫到輸出位置
            output.TagName = null;
            output.TagMode = TagMode.StartTagAndEndTag;

            var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);

            // <select>
            var select = new TagBuilder("select");
            select.Attributes["name"] = fullName;
            select.Attributes["class"] = "col-12 js-select2-tag";
            select.Attributes["multiple"] = string.Empty;
            select.TagRenderMode = TagRenderMode.Normal;
            output.Content.AppendHtml(select);

            // 預選的 tag id (給 tag-select2.js 讀取)
            if (For.Model is IEnumerable<string> ids)
            {
                foreach (var id in ids)
                {
                    if (string.IsNullOrEmpty(id)) continue;
                    var div = new TagBuilder("div");
                    div.Attributes["class"] = "TagIds d-none";
                    div.InnerHtml.Append(id);
                    output.Content.AppendHtml(div);
                }
            }
        }
    }
}
