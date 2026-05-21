using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagFuncGroupId : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                var uow = filterContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                controller.ViewBag.FuncGroupId = uow.FuncGroupRepo.All().OrderBy(p => p.DisplaySeq);
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
