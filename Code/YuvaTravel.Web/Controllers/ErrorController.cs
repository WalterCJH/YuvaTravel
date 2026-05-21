using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace YuvaTravel.Web.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorController : Controller
    {
        [Route("Error")]
        [Route("Error/{statusCode:int}")]
        public IActionResult Index(int? statusCode = null)
        {
            // 必加:錯誤頁本身不該被索引
            Response.Headers["X-Robots-Tag"] = "noindex, nofollow";

            // 判斷是否為內部 ReExecute 轉進來 (非使用者直接打 /Error/xxx)
            var reExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (reExecuteFeature is null)
            {
                // 使用者直接訪問 /Error 或 /Error/500 → 視為不存在的網址
                Response.StatusCode = 404;
                return View("NotFound");
            }

            // 正常情況:誠實回傳真實狀態碼
            int code = statusCode ?? 500;
            Response.StatusCode = code;

            return code switch
            {
                404 => View("NotFound"),
                403 => View("Forbidden"),
                500 => View("ServerError"),
                _   => View("GenericError")
            };
        }
    }
}
