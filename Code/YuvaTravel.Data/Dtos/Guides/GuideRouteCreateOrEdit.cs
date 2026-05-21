using System;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.Guides
{
    public class GuideRouteCreateOrEdit
    {
        [Display(Name = "ID")]
        public Guid? GuideRouteId { get; set; }

        [Required(ErrorMessage = StrText.RequireSelectd)]
        [Display(Name = "嚮導")]
        public Guid? GuideId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "路線標題")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "說明")]
        public string Description { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "時長 (例 半日 · 4 小時)")]
        public string Duration { get; set; }

        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "連結")]
        public string Url { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
