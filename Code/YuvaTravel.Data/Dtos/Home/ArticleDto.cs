using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Home
{
    public class ArticleDto : BaseArticleDto
    {
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Content { get; set; }

        public Guid? ArticleId { get; set; }
        public Guid? ArticleCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryUrl { get; set; }
        public string DatePublished { get; set; }
        public string ImageGoToPlay { get; set; }
        //public string ImageAdsPath { get; set; }

        public string Author { get; set; }
        public string AuthorBio { get; set; }
        public List<GuideRegionRef> GuideRegions { get; set; } = new List<GuideRegionRef>();
        public string GuideRegionStr { get { return string.Join(" · ", GuideRegions.Select(c => c.Name).ToList()); } }
        public int? ReadTimeMin { get; set; }
        public int SerialNumber { get; set; }

        public List<BaseArticleDto> RecommendedArticles { get; set; }
        public List<BaseArticleDto> RelatedArticles { get; set; } = new List<BaseArticleDto>();
        public BaseArticleDto PrevArticle { get; set; }
        public BaseArticleDto NextArticle { get; set; }

        public ArticleDto()
        {
            RecommendedArticles = new List<BaseArticleDto>();
        }
    }
}
