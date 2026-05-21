using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Web.Helpers
{
    public class DataGenHelper
    {
        public static async Task<ArticleDto> GetArticles(
            IArticleTagRepository repoArticleTag,
            IArticleSDCommonQuestionRepository repoArticleSDCommonQuestion,
            Article article, string host, string goRegister)
        {
            var dto = new ArticleDto();
            dto.MetaTitle = article.MetaTitle;
            dto.MetaDescription = article.MetaDescription;
            dto.ImagePath = article.ImagePath;
            dto.ImageMobilePath = article.ImageHomePath;
            dto.ImageAlt = article.ImageAlt;
            dto.ImageTitle = article.ImageTitle;
            dto.Title = article.Title;
            dto.Content = article.ContentAll;
            dto.Description = article.Description;
            dto.Tags = repoArticleTag.GetArticleTags(article.ArticleTags);
            dto.ArticleId = article.ArticleId;
            var category = article.ArticleCategoryArticles.FirstOrDefault()?.ArticleCategory;
            dto.ArticleCategoryId = category?.ArticleCategoryId;
            dto.CategoryName = category?.Name;
            dto.CategoryCode = category?.Code;
            dto.CategoryUrl = StrPath.ArticleCategory(host, dto.CategoryCode);

            dto.DatePublished = article.UserLog.CreateTime?.ToString("yyyy-MM-ddTHH:mm:ss+08:00");
            dto.ImageGoToPlay = $"{goRegister}&m=Image_JoinUs";
            dto.EventLocation = article.EventLocation;
            dto.EventStarNum = article.EventStarNum;
            dto.EventCount = article.EventCount;
            dto.EventPrice = article.EventPrice;
            dto.EventDiscount = article.EventDiscount;
            dto.EventDiscountValue = article.EventDiscountValue;
            dto.ArticleSDCSs = await repoArticleSDCommonQuestion.QueryShowFromArticleId(article.ArticleId);

            return dto;
        }

        public static List<BaseArticleDto> GetBaseArticlesFromArticles(
            List<Article> articles,
            IArticleTagRepository repoArticleTag,
            string host, bool showPublishDate, bool showViews)
        {
            var dto = new List<BaseArticleDto>();
            for (int i = 0; i < articles.Count; i++)
            {
                var baseArtDto = new BaseArticleDto();
                dto.Add(baseArtDto);
                baseArtDto.C = articles[i].GetCodeOrId;
                baseArtDto.Url = StrPath.Article(host, baseArtDto.C);
                baseArtDto.ImagePath = articles[i].ImageHomePath;
                baseArtDto.ImageMobilePath = articles[i].ImageMobilePath;
                baseArtDto.ImageAlt = articles[i].ImageAlt;
                baseArtDto.ImageTitle = articles[i].ImageTitle;
                baseArtDto.Title = articles[i].Title;
                baseArtDto.Description = articles[i].Description;
                baseArtDto.ShowPublishDate = showPublishDate;
                baseArtDto.OnlineTime = articles[i].OnlineTime;
                baseArtDto.ShowViews = showViews;
                baseArtDto.Views = articles[i].Views;
                baseArtDto.ReadTimeMin = articles[i].ReadTimeMin;
                baseArtDto.Tags = repoArticleTag.GetArticleTags(articles[i].ArticleTags);
                baseArtDto.EventLocation = articles[i].EventLocation;
                baseArtDto.EventStarNum = articles[i].EventStarNum;
                baseArtDto.EventCount = articles[i].EventCount;
                baseArtDto.EventPrice = articles[i].EventPrice;
                baseArtDto.EventDiscount = articles[i].EventDiscount;
                baseArtDto.EventDiscountValue = articles[i].EventDiscountValue;
            }
            return dto;
        }

        public static async Task<List<BaseArticleDto>> GetRecommendArticlesForAjax(
            IArticleTagRepository repoArticleTag,
            IQueryable<Article> allArticles, Guid? articleCategoryId, Guid? articleId)
        {
            var query = allArticles;
            if (articleCategoryId != null)
                query = query.Where(p => p.ArticleCategoryArticles.Any(a => a.ArticleCategoryId == articleCategoryId));
            if (articleId != null)
                query = query.Where(p => p.ArticleId != articleId);

            var articles = await query.Take(9).ToListAsync();

            if (articles.Count < 9)
            {
                var article = allArticles.FirstOrDefault(p => p.ArticleId == articleId);
                if (article != null)
                {
                    var tagIds = article.ArticleTags.Select(p => p.Tag.TagId).ToList();
                    foreach (var tagId in tagIds)
                    {
                        var tempArticles = allArticles.Where(p => p.ArticleTags.Any(a => a.Tag.TagId == tagId)).Take(9).ToList();
                        foreach (var tempArticle in tempArticles)
                        {
                            if (!articles.Contains(tempArticle) && articleId != tempArticle.ArticleId)
                            {
                                articles.Add(tempArticle);
                                if (articles.Count == 9) break;
                            }
                        }
                        if (articles.Count == 9) break;
                    }
                }
            }

            return articles.Select(s => new BaseArticleDto
            {
                C = (!string.IsNullOrWhiteSpace(s.Code)) ? s.Code : s.ArticleId.ToString(),
                ImagePath = s.ImagePath,
                ImageMobilePath = s.ImageMobilePath,
                ImageAlt = s.ImageAlt,
                ImageTitle = s.ImageTitle,
                Title = s.Title,
                Description = s.Description,
                OnlineTime = s.OnlineTime,
                Views = s.ArticleViews.Count(),
                Tags = repoArticleTag.GetArticleTags(s.ArticleTags)
            }).ToList();
        }
    }
}
