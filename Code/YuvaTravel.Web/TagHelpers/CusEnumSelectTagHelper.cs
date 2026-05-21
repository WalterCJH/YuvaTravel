using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// Enum 下拉選單。輸出 &lt;select&gt;。
    /// 用法：
    ///   &lt;cus-enum-select asp-for="MyEnumProp" /&gt;
    ///       自動列舉 enum 所有值（適合單純 enum 欄位）
    ///   &lt;cus-enum-select asp-for="X" asp-items="@(new SelectList(ViewBag.X, "Value", "Key"))" /&gt;
    ///       使用外部過濾後的清單（適合 server-side 動態篩選的 enum）
    ///   &lt;cus-enum-select asp-for="X" default-text="(請選擇)" /&gt;
    ///       自訂預設選項文字；傳空字串可關閉預設選項
    /// </summary>
    [HtmlTargetElement("cus-enum-select", Attributes = "asp-for")]
    public class CusEnumSelectTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; } = default!;

        [HtmlAttributeName("asp-items")]
        public IEnumerable<SelectListItem>? Items { get; set; }

        [HtmlAttributeName("default-text")]
        public string DefaultText { get; set; } = "(請選擇)";

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

            var selectedValue = GetSelectedValue();
            var items = ResolveItems();

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(DefaultText))
            {
                sb.Append($"<option value=\"\">{WebUtility.HtmlEncode(DefaultText)}</option>");
            }
            foreach (var item in items)
            {
                var selected = item.Value == selectedValue ? " selected=\"selected\"" : "";
                sb.Append($"<option value=\"{WebUtility.HtmlEncode(item.Value)}\"{selected}>{WebUtility.HtmlEncode(item.Text)}</option>");
            }
            output.Content.SetHtmlContent(sb.ToString());
        }

        private IEnumerable<SelectListItem> ResolveItems()
        {
            if (Items != null) return Items;

            var enumType = Nullable.GetUnderlyingType(For.Metadata.ModelType) ?? For.Metadata.ModelType;
            if (!enumType.IsEnum) return Enumerable.Empty<SelectListItem>();

            return Enum.GetValues(enumType).Cast<object>()
                .Select(v => new SelectListItem
                {
                    Value = ((int)v).ToString(),
                    Text = v.ToString() ?? string.Empty
                });
        }

        private string? GetSelectedValue()
        {
            if (For.Model == null) return null;
            return For.Model is Enum
                ? Convert.ToInt32(For.Model).ToString()
                : For.Model.ToString();
        }
    }
}
