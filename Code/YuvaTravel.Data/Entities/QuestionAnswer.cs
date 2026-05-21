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
    public class QuestionAnswer
    {
        [Key]
        [Display(Name = "ID")]
        public Guid QuestionAnswerId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(200)]
        [Display(Name = "Url(絕對位置 或 相對位置)")]
        public string Url { get; set; }

        [MaxLength(200)]
        [Display(Name = "標題")]
        public string Title { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(4000)]
        [Display(Name = "內容")]
        public string Content { get; set; }

        [Display(Name = "顯示順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public QuestionAnswer()
        {
            UserLog = new UserLog();
        }
    }
}
