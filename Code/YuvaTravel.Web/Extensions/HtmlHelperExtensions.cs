using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace YuvaTravel.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// ASP.NET Core equivalent of MVC5 Html.EnumDropDownListFor.
        /// Works for enum types and nullable bool types.
        /// </summary>
        public static IHtmlContent EnumDropDownListFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null)
        {
            return EnumDropDownListFor(htmlHelper, expression, null, htmlAttributes);
        }

        public static IHtmlContent EnumDropDownListFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string optionLabel,
            object htmlAttributes = null)
        {
            var type = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);

            IEnumerable<SelectListItem> items;

            if (type == typeof(bool))
            {
                items = new List<SelectListItem>
                {
                    new SelectListItem { Text = "是", Value = "True" },
                    new SelectListItem { Text = "否", Value = "False" }
                };
            }
            else if (type.IsEnum)
            {
                items = htmlHelper.GetEnumSelectList(type);
            }
            else
            {
                items = Enumerable.Empty<SelectListItem>();
            }

            return htmlHelper.DropDownListFor(expression, items, optionLabel, htmlAttributes);
        }
    }
}
