using Microsoft.AspNetCore.Mvc.Filters;

namespace YuvaTravel.Infrastructure.ActionFilters
{
    // ASP.NET Core 的壓縮應改用 ResponseCompression 中介軟體：
    // builder.Services.AddResponseCompression(); app.UseResponseCompression();
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Response.Filter 在 ASP.NET Core 不存在，壓縮改由中介軟體處理
        }
    }
}
