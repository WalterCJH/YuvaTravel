using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Home
{
    public class SearchPageDto
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        public List<HotKeywordItemDto> HotKeywords { get; set; } = new List<HotKeywordItemDto>();
        public List<TagCloudItemDto> HotTags { get; set; } = new List<TagCloudItemDto>();
        public List<BaseArticleDto> EditorPicks { get; set; } = new List<BaseArticleDto>();
    }

    public class HotKeywordItemDto
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
