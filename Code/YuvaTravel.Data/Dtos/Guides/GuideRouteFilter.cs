using System;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Guides
{
    public class GuideRouteFilter
    {
        [Display(Name = "嚮導")]
        public Guid? GuideId { get; set; }

        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [RegularExpression(@"(DisplaySeq|Title)")]
        public string SortBy { get; set; } = "DisplaySeq";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;
    }
}
