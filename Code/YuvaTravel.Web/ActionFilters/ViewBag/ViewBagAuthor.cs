using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagAuthor : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                var uow = filterContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                var list = new Dictionary<string, string>();
                var users = uow.ArticleRepo.All().GroupBy(p => p.UserLog.CreateUserId);
                foreach (var user in users)
                {
                    if (user.Key != null)
                        list[user.Key] = user.Key;
                }
                controller.ViewBag.Author = list.OrderBy(p => p.Key);
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
