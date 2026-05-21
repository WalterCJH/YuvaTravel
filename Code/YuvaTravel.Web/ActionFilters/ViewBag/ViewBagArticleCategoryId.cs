using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagArticleCategoryId : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                var uow = filterContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                controller.ViewBag.ArticleCategoryId = uow.ArticleCategoryRepo.All().OrderBy(p => p.DisplaySeq);
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
