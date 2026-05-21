using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Home
{
    public class RecruitAgentDto
    {
        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "暱稱")]
        public string Nick { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(20, ErrorMessage = StrText.OverLength)]
        [Display(Name = "電話")]
        public string Phone { get; set; }

        [Display(Name = "Telegram ID")]
        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        public string TelegramID { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "Line ID")]
        public string LineID { get; set; }

        [Display(Name = "其他")]
        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        public string Other { get; set; }

        [Display(Name = "已聯繫")]
        public bool IsContact { get; set; }

        [Display(Name = "已聯繫時間")]
        public DateTime? ContactTime { get; set; }

    }
}
