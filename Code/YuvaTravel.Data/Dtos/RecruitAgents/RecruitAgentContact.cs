using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Base.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Data.Dtos.Home;

namespace YuvaTravel.Data.Dtos.RecruitAgents
{
    public class RecruitAgentContact
    {
        [Display(Name = "ID")]
        public Guid RecruitAgentId { get; set; }

        [Display(Name = "暱稱")]
        public string Nick { get; set; }

        [Display(Name = "電話")]
        public string Phone { get; set; }

        [Display(Name = "Telegram ID")]
        public string TelegramID { get; set; }

        [Display(Name = "Line ID")]
        public string LineID { get; set; }

        [Display(Name = "其他")]
        public string Other { get; set; }

        [Display(Name = "已聯繫")]
        public bool IsContact { get; set; }

        [Display(Name = "已聯繫時間")]
        public DateTime? ContactTime { get; set; }

    }
}
