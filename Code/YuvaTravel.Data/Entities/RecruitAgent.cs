using YuvaTravel.Data.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Entities
{
    public class RecruitAgent
    {
        [Key]
        [Display(Name = "ID")]
        public Guid RecruitAgentId { get; set; }

        [MaxLength(30)]
        [Display(Name = "暱稱")]
        public string Nick { get; set; }

        [MaxLength(20)]
        [Display(Name = "電話")]
        public string Phone { get; set; }

        [MaxLength(100)]
        [Display(Name = "Telegram ID")]
        public string TelegramID { get; set; }

        [MaxLength(100)]
        [Display(Name = "Line ID")]
        public string LineID { get; set; }

        [MaxLength(1000)]
        [Display(Name = "其他")]
        public string Other { get; set; }

        public UserLog UserLog { get; set; }

        [Display(Name = "填寫時間")]
        public string FillTime
        {
            get
            {
                return UserLog?.CreateTime?.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        [Display(Name = "已聯繫")]
        public bool IsContact { get; set; }

        [Display(Name = "聯繫時間")]
        public DateTime? ContactTime { get; set; }

        public RecruitAgent()
        {
            UserLog = new UserLog();
        }
    }
}
