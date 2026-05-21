using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.Base;
using YuvaTravel.Web.Extensions;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagUserGroupId : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                var userData = filterContext.HttpContext.Session.Get<UserData>(StrSession.AdminUserData);
                if (userData == null) return;

                var uow = filterContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                var userGroupId = userData.UserGroupId;
                var data = uow.UserGroupRepo.All();
                if (userGroupId?.ToLower() != "systemg")
                {
                    data = data.Where(p => p.UserGroupId.ToLower() != "systemg");
                }
                controller.ViewBag.UserGroupId = data.OrderBy(p => p.Name);
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
