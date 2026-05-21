using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YuvaTravel.Data.Entities
{
    public class GuideTag
    {
        [Key]
        [Display(Name = "ID")]
        public Guid GuideTagId { get; set; }

        public Guid GuideId { get; set; }
        [ForeignKey("GuideId")]
        public virtual Guide Guide { get; set; }

        public Guid TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
