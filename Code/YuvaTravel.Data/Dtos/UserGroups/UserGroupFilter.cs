using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Data.Dtos.UserGroups
{
    public class UserGroupFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "權限等級")]
        public AuthorizeLevel? AuthorizeLevel { get; set; }

        [RegularExpression(@"(UserGroupId|AuthorizeLevel|Name)")]
        public string SortBy { get; set; } = "AuthorizeLevel";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "DESC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

    }
}
