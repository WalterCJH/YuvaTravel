using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagSalutation : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                controller.ViewBag.Salutations = new List<string> { StrText.Mr, StrText.Mrs };
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
