using System.Collections.Generic;

namespace YuvaTravel.Data.Dtos.Home
{
    public class HomePageDto
    {
        public List<BaseArticleDto> EventArticles { get; set; } = new List<BaseArticleDto>();
        public List<BaseArticleDto> NewestArticles { get; set; } = new List<BaseArticleDto>();
        public List<BaseArticleDto> ViewArticles { get; set; } = new List<BaseArticleDto>();

        public List<BaseArticleDto> MosaicArticles { get; set; } = new List<BaseArticleDto>();
        public List<BaseArticleDto> FeaturedArticles { get; set; } = new List<BaseArticleDto>();
        public List<ThemeCategoryDto> ThemeCategories { get; set; } = new List<ThemeCategoryDto>();
        public List<RegionItemDto> Regions { get; set; } = new List<RegionItemDto>();
        public List<GuideCardDto> FeaturedGuides { get; set; } = new List<GuideCardDto>();
    }
}
