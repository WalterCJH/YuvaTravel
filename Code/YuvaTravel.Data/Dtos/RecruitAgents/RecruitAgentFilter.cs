using YuvaTravel.Base.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Dtos.RecruitAgents
{
    public class RecruitAgentFilter
    {
        [Display(Name = "已聯繫")]
        public bool IsContact { get; set; }

        [Display(Name = "暱稱")]
        public string Nick { get; set; }

        [Display(Name = "電話")]
        public string Phone { get; set; }

        [Display(Name = "Telegram ID")]
        public string TelegramID { get; set; }

        [Display(Name = "Line ID")]
        public string LineID { get; set; }

        [RegularExpression(@"(Nick|Phone|UserLog.CreateTime|ContactTime)")]
        public string SortBy { get; set; } = "UserLog.CreateTime";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "DESC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

    }
}
