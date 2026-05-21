using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace YuvaTravel.Web.TagHelpers
{
    /// <summary>
    /// 輸出 model property 的 [Display(Name=...)]（等同 Html.DisplayNameFor），不包覆任何 element。
    /// 用法：
    ///   &lt;cus-display-name asp-for="Name" /&gt;
    ///       使用 page model 的屬性
    ///   &lt;cus-display-name for-type="@typeof(HotKeyword)" property="IsActive" /&gt;
    ///       依型別 + 屬性名稱查 metadata（適用 list 表頭場景，page model 不是 entity 本身時）
    /// </summary>
    [HtmlTargetElement("cus-display-name", Attributes = "asp-for")]
    [HtmlTargetElement("cus-display-name", Attributes = "for-type,property")]
    public class CusDisplayNameTagHelper : TagHelper
    {
        private readonly IModelMetadataProvider _metadataProvider;

        public CusDisplayNameTagHelper(IModelMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression? For { get; set; }

        [HtmlAttributeName("for-type")]
        public Type? ForType { get; set; }

        [HtmlAttributeName("property")]
        public string? Property { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            output.TagMode = TagMode.StartTagAndEndTag;

            string displayName;
            if (For != null)
            {
                displayName = For.Metadata.DisplayName ?? For.Name;
            }
            else if (ForType != null && !string.IsNullOrEmpty(Property))
            {
                var metadata = _metadataProvider.GetMetadataForProperties(ForType)
                    .FirstOrDefault(p => p.PropertyName == Property);
                displayName = metadata?.DisplayName ?? Property;
            }
            else
            {
                displayName = string.Empty;
            }

            output.Content.SetContent(displayName);
        }
    }
}
