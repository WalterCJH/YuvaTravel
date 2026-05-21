using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Data.Dtos.Subscribers
{
    public class SubscriberFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "啟用")]
        public WhetherType? IsActive { get; set; }

        [RegularExpression(@"(SubscribeTime|Email|IsActive)")]
        public string SortBy { get; set; } = "SubscribeTime";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "DESC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;
    }
}
