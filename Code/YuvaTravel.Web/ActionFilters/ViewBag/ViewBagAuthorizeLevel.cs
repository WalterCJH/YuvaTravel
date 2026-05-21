using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Base;
using YuvaTravel.Web.Extensions;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagAuthorizeLevel : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                var userData = filterContext.HttpContext.Session.Get<UserData>(StrSession.AdminUserData);
                if (userData == null) return;

                var authroizeLevel = userData.AuthorizeLevel;
                var list = new Dictionary<string, int>();
                foreach (var value in Enum.GetValues(typeof(AuthorizeLevel)))
                {
                    AuthorizeLevel authorizeLevel = (AuthorizeLevel)value;
                    if (authorizeLevel <= authroizeLevel)
                    {
                        list[authorizeLevel.ToString()] = (int)authorizeLevel;
                    }
                }
                controller.ViewBag.AuthorizeLevel = list.OrderByDescending(p => p.Value);
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
