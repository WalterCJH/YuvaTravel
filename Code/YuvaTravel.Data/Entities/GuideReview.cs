using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class GuideReview
    {
        [Key]
        [Display(Name = "ID")]
        public Guid GuideReviewId { get; set; }

        public Guid GuideId { get; set; }
        [ForeignKey("GuideId")]
        public virtual Guide Guide { get; set; }

        [MaxLength(50)]
        [Display(Name = "旅人姓名")]
        public string ReviewerName { get; set; }

        [MaxLength(50)]
        [Display(Name = "旅人來自")]
        public string ReviewerFrom { get; set; }

        [Display(Name = "星等 (1-5)")]
        public int Stars { get; set; }

        [MaxLength(1000)]
        [Display(Name = "回饋內容")]
        public string Content { get; set; }

        [Display(Name = "旅遊月份")]
        public DateTime? TripDate { get; set; }

        [Display(Name = "旅遊天數")]
        public int? TripDays { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public GuideReview()
        {
            UserLog = new UserLog();
        }
    }
}
