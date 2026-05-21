using System;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.Guides
{
    public class GuideReviewCreateOrEdit
    {
        [Display(Name = "ID")]
        public Guid? GuideReviewId { get; set; }

        [Required(ErrorMessage = StrText.RequireSelectd)]
        [Display(Name = "嚮導")]
        public Guid? GuideId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "旅人姓名")]
        public string ReviewerName { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "旅人來自")]
        public string ReviewerFrom { get; set; }

        [Range(1, 5)]
        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "星等 (1–5)")]
        public int Stars { get; set; } = 5;

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(1000, ErrorMessage = StrText.OverLength)]
        [Display(Name = "回饋內容")]
        public string Content { get; set; }

        [Display(Name = "旅遊月份")]
        public DateTime? TripDate { get; set; }

        [Display(Name = "旅遊天數")]
        public int? TripDays { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
