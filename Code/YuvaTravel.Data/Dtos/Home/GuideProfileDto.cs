using System;
using System.Collections.Generic;

namespace YuvaTravel.Data.Dtos.Home
{
    public class GuideProfileDto
    {
        public Guid GuideId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public List<GuideRegionRef> Regions { get; set; } = new List<GuideRegionRef>();
        public int YearsOfExperience { get; set; }
        public string Languages { get; set; }
        public string ServiceFormat { get; set; }
        public string Bio { get; set; }
        public string LongBio { get; set; }
        public string QuoteText { get; set; }
        public string QuoteBy { get; set; }
        public string PortraitUrl { get; set; }
        public string CoverUrl { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public int? MinPeople { get; set; }
        public int? MaxPeople { get; set; }
        public string ResponseTime { get; set; }
        public int? StartingPrice { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
        public List<GuideRouteDto> Routes { get; set; } = new List<GuideRouteDto>();
        public List<GuideReviewDto> Reviews { get; set; } = new List<GuideReviewDto>();
        public List<GuideFaqItemDto> Faqs { get; set; } = new List<GuideFaqItemDto>();
        public List<BaseArticleDto> Articles { get; set; } = new List<BaseArticleDto>();
        public int ArticleTotalCount { get; set; }
        public List<GuideCardDto> SimilarGuides { get; set; } = new List<GuideCardDto>();
    }

    public class GuideRouteDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Url { get; set; }
        public int DisplaySeq { get; set; }
    }

    public class GuideReviewDto
    {
        public string ReviewerName { get; set; }
        public string ReviewerFrom { get; set; }
        public int Stars { get; set; }
        public string Content { get; set; }
        public DateTime? TripDate { get; set; }
        public int? TripDays { get; set; }

        public string Display
        {
            get
            {
                var parts = new List<string>();
                if (TripDate.HasValue) parts.Add(TripDate.Value.ToString("yyyy.MM"));
                if (!string.IsNullOrEmpty(City)) parts.Add(City);
                if (TripDays.HasValue) parts.Add($"{TripDays} 天");
                return string.Join(" · ", parts);
            }
        }

        public string City { get; set; }
    }

    public class GuideFaqItemDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
