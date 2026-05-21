using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.ArticleCategories
{
    public class ArticleCategoryFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [RegularExpression(@"(DisplaySeq|Code|Name)")]
        public string SortBy { get; set; } = "DisplaySeq";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;
    }
}
