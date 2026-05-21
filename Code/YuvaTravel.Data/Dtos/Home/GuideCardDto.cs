using System;
using System.Collections.Generic;

namespace YuvaTravel.Data.Dtos.Home
{
    public class GuideCardDto
    {
        public Guid GuideId { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public int YearsOfExperience { get; set; }
        public string Languages { get; set; }
        public string Bio { get; set; }
        public string PortraitUrl { get; set; }
        public string CoverUrl { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public int ArticleCount { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<GuideRegionRef> Regions { get; set; } = new List<GuideRegionRef>();
    }

    public class GuideRegionRef
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
