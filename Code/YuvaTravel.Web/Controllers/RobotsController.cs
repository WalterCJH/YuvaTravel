using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace YuvaTravel.Web.Controllers
{
    public class RobotsController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public RobotsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("robots.txt")]
        [ResponseCache(Duration = 3600)]
        public IActionResult Index()
        {
            var sb = new StringBuilder();

            if (_env.IsProduction())
            {
                sb.AppendLine("User-agent: *");
                sb.AppendLine("Disallow: /Admin/");
                sb.AppendLine("Disallow: /admin/");
                sb.AppendLine("Disallow: /Account/");
                sb.AppendLine("Disallow: /LazyLoad/");
                sb.AppendLine("Disallow: /Error/");
                sb.AppendLine("Disallow: /Search");
                sb.AppendLine("Disallow: /search");
                sb.AppendLine("Disallow: /*?keyword=");
                sb.AppendLine("Disallow: /*?s=");
                sb.AppendLine();
                sb.AppendLine("# 允許 Google 圖片爬蟲索引圖片");
                sb.AppendLine("User-agent: Googlebot-Image");
                sb.AppendLine("Allow: /Images/");
                sb.AppendLine();
                sb.AppendLine($"Sitemap: {Request.Scheme}://{Request.Host}/sitemap.xml");
            }
            else
            {
                // 開發 / Staging:完全禁止索引
                sb.AppendLine("User-agent: *");
                sb.AppendLine("Disallow: /");
            }

            return Content(sb.ToString(), "text/plain", Encoding.UTF8);
        }
    }
}
