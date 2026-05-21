using System;

namespace YuvaTravel.Data.Dtos.Home
{
    public class RegionItemDto
    {
        public Guid RegionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int ArticleCount { get; set; }
        public int GuideCount { get; set; }
        public int DisplaySeq { get; set; }
    }
}
