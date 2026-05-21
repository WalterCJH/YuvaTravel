using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Dtos.UrlReferrerCodes
{
    public class UrlReferrerCodeFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [RegularExpression(@"(Code|Name)")]
        public string SortBy { get; set; } = "Code";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

    }
}
