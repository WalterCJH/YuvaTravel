using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Dtos.WebBanners
{
    public class WebBannerFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "期間")]
        public string RangeDate { get; set; }

        [RegularExpression(@"(DisplaySeq|Title|ImageTitle|ImageAlt)")]
        public string SortBy { get; set; } = "DisplaySeq";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

    }
}
