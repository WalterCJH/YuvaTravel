using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagHotKeywords : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                var uow = filterContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                controller.ViewBag.HotKeywords = uow.HotKeywordRepo.All()
                    .Where(p => p.IsActive).OrderBy(p => p.DisplaySeq).Select(p => p.Name).ToList();
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
