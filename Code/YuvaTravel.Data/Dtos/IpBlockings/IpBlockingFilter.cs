using YuvaTravel.Base.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Dtos.IpBlockings
{
    public class IpBlockingFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "IP")]
        public string IpAddress { get; set; }

        [Display(Name = "啟用")]
        public WhetherType? IsActive { get; set; }

        [RegularExpression(@"(IpAddress|UserLog.CreateTime)")]
        public string SortBy { get; set; } = "UserLog.CreateTime";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "DESC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

    }
}
