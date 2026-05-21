using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Home
{
    public class CategoryListDto
    {
        [Display(Name = "代碼")]
        public string Id { get; set; }

        [Display(Name = "搜尋關鍵字")]
        public string S { get; set; }

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

        [Display(Name = "排序方式")]
        public string SortBy { get; set; } = "newest";

        public string CurrentName { get; set; }
        public string CurrentNameEn { get; set; }
        public string CurrentDescription { get; set; }
        public int CurrentArticleCount { get; set; }
        public int CurrentGuideCount { get; set; }

        public List<ThemeCategoryDto> AllCategories { get; set; } = new List<ThemeCategoryDto>();
        public List<RegionItemDto> AllRegions { get; set; } = new List<RegionItemDto>();
        public List<BaseArticleDto> Articles { get; set; } = new List<BaseArticleDto>();
        public List<TagDto> HotTags { get; set; } = new List<TagDto>();

        public int TotalArticles { get; set; }
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
    }
}
