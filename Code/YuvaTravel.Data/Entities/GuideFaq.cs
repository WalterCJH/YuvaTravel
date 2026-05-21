using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class GuideFaq
    {
        [Key]
        [Display(Name = "ID")]
        public Guid GuideFaqId { get; set; }

        public Guid GuideId { get; set; }
        [ForeignKey("GuideId")]
        public virtual Guide Guide { get; set; }

        [MaxLength(200)]
        [Display(Name = "問題")]
        public string Question { get; set; }

        [Display(Name = "回覆")]
        public string Answer { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public GuideFaq()
        {
            UserLog = new UserLog();
        }
    }
}
