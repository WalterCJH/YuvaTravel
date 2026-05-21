using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Home
{
    public class GuidesPageDto
    {
        [Display(Name = "城市篩選")]
        public string Id { get; set; }

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

        public int TotalGuides { get; set; }
        public int PageSize { get; set; } = 9;
        public int TotalPages { get; set; }

        public int CityCount { get; set; }
        public List<string> Languages { get; set; } = new List<string>();
        public List<CityChipDto> Cities { get; set; } = new List<CityChipDto>();

        public GuideCardDto FeaturedGuide { get; set; }
        public List<GuideCardDto> Guides { get; set; } = new List<GuideCardDto>();
    }

    public class CityChipDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Count { get; set; }
    }
}
