using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YuvaTravel.Web.Helpers;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Controllers
{
    public class LazyLoadController : Controller, IExceptionFilter
    {
        private readonly ILogger<LazyLoadController> logger;
        IUnitOfWork uow;
        string Host = "";

        public LazyLoadController(IUnitOfWork unitOfWork, ILogger<LazyLoadController> logger)
        {
            uow = unitOfWork;
            this.logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            Host = $"{Request.Scheme}://{Request.Host}";
        }

        public void OnException(ExceptionContext context)
        {
            var absoluteUri = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            logger.LogError(context.Exception, "LazyLoad 未處理例外 UrlPath:{Url}", absoluteUri);
            context.Result = RedirectToAction("Error", "Home");
            context.ExceptionHandled = true;
        }

        #region 文章

        [HttpPost]
        public async Task<IActionResult> _RecommendedArticles(Guid? ArticleCategoryId, Guid? ArticleId)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var dto = await DataGenHelper.GetRecommendArticlesForAjax(
                    uow.ArticleTagRepo, uow.ArticleRepo.ActiveArticleANT(), ArticleCategoryId, ArticleId);
                return PartialView("/Views/Home/_RecommendedArticle.cshtml", dto);
            }
            else
            {
                return Content("Fail");
            }
        }

        #endregion
    }
}
