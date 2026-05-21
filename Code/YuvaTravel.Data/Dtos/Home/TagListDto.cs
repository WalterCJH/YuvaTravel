using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Home
{
    public class TagListDto
    {
        [Display(Name = "標籤名稱")]
        public string T { get; set; }

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

        [Display(Name = "排序方式")]
        public string SortBy { get; set; } = "newest";

        public string CurrentName { get; set; }
        public string CurrentDescription { get; set; }
        public int CurrentArticleCount { get; set; }
        public int CurrentGuideCount { get; set; }

        public List<TagDto> RelatedTags { get; set; } = new List<TagDto>();
        public List<TagCloudItemDto> AllTags { get; set; } = new List<TagCloudItemDto>();
        public List<BaseArticleDto> Articles { get; set; } = new List<BaseArticleDto>();
        public List<GuideCardDto> Guides { get; set; } = new List<GuideCardDto>();

        public int TotalArticles { get; set; }
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
    }

    public class TagCloudItemDto
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public string Url { get; set; }

        public string Rank
        {
            get
            {
                if (Count >= 25) return "xl";
                if (Count >= 18) return "lg";
                if (Count >= 12) return "md";
                return "sm";
            }
        }
    }
}
