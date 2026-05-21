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
    public class HotKeyword
    {
        [Key]
        [Display(Name = "ID")]
        public Guid HotKeywordId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [MaxLength(20)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public HotKeyword()
        {
            UserLog = new UserLog();
        }
    }
}
