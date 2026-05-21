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
    public class IpBlocking
    {
        [Key]
        [Display(Name = "ID")]
        public Guid IpBlockingId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [MaxLength(100)]
        [Display(Name = "IP位置")]
        public string IpAddress { get; set; }

        [Display(Name = "是否為不完整IP")]
        public bool IsIncompleteIP { get; set; }

        [MaxLength(500)]
        [Display(Name = "備註")]
        public string Memo { get; set; }

        public UserLog UserLog { get; set; }

        [Display(Name = "封鎖時間")]
        public string BlockTime
        {
            get
            {
                return UserLog?.CreateTime?.ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
        }

        public IpBlocking()
        {
            UserLog = new UserLog();
        }
    }
}
