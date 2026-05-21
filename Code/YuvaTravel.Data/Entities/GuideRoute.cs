using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class GuideRoute
    {
        [Key]
        [Display(Name = "ID")]
        public Guid GuideRouteId { get; set; }

        public Guid GuideId { get; set; }
        [ForeignKey("GuideId")]
        public virtual Guide Guide { get; set; }

        [MaxLength(50)]
        [Display(Name = "路線標題")]
        public string Title { get; set; }

        [MaxLength(500)]
        [Display(Name = "說明")]
        public string Description { get; set; }

        [MaxLength(50)]
        [Display(Name = "時長")]
        public string Duration { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(200)]
        [Display(Name = "連結")]
        public string Url { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public GuideRoute()
        {
            UserLog = new UserLog();
        }
    }
}
