using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagOgType : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                controller.ViewBag.OgType = Enum.GetValues(typeof(OgType))
                    .Cast<OgType>().ToDictionary(t => t.ToString(), t => t.ToString());
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
