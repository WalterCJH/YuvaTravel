using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Dtos.FuncGroups
{
    public class FuncGroupFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "使用者群組")]
        public string FuncGroupId { get; set; }


        [RegularExpression(@"(FuncGroupId|Name|DisplaySeq)")]
        public string SortBy { get; set; } = "DisplaySeq";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

    }
}
