using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Data.Dtos.Regions
{
    public class RegionFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "啟用")]
        public WhetherType? IsActive { get; set; }

        [RegularExpression(@"(DisplaySeq|Code|Name)")]
        public string SortBy { get; set; } = "DisplaySeq";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;
    }
}
