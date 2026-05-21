using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.ActionFilters.ViewBag
{
    public class ViewBagProgramId : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult && filterContext.Controller is Controller controller)
            {
                var uow = filterContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                var list = new List<string>();
                foreach (var prg in Enum.GetNames(typeof(ProgramId)))
                {
                    if (!uow.FuncProgramRepo.All().Any(p => p.FuncProgramId == prg) && prg != ProgramId.Admin.ToString())
                    {
                        list.Add(prg);
                    }
                }
                controller.ViewBag.ProgramId = list;
            }
            base.OnResultExecuting(filterContext);
        }
    }
}
