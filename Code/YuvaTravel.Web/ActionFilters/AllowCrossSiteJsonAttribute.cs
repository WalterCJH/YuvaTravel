using Microsoft.AspNetCore.Mvc.Filters;

namespace YuvaTravel.Web.ActionFilters
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            base.OnActionExecuting(filterContext);
        }
    }
}
