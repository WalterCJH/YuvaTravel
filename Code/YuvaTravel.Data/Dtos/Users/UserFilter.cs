using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Data.Dtos.Users
{
    public class UserFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "使用者群組")]
        public string UserGroupId { get; set; }

        //[Display(Name = "啟用")]
        //public bool IsActive { get; set; } = true;

        [Display(Name = "啟用")]
        public WhetherType? IsActive { get; set; }

        [RegularExpression(@"(UserId|LastName)")]
        public string SortBy { get; set; } = "UserId";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

    }
}
