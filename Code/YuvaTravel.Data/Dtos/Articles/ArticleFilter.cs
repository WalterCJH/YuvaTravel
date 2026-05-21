using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Data.Dtos.Articles
{
    public class ArticleFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "作者")]
        public string Author { get; set; }

        [Display(Name = "文章類別")]
        public Guid? ArticleCategoryId { get; set; }

        [Display(Name = "審核狀態")]
        public ArticleReviewType? ArticleReviewType { get; set; }

        [Display(Name = "置頂")]
        public WhetherType? IsTop { get; set; }

        [Display(Name = "精選文章")]
        public WhetherType? IsFeaturedArticle { get; set; }

        [RegularExpression(@"(DisplaySeq|Views|ArticleCategory.Name|Code|Title|FeaturedArticleDisplaySeq)")]
        public string SortBy { get; set; } = "DisplaySeq";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "DESC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

    }
}
