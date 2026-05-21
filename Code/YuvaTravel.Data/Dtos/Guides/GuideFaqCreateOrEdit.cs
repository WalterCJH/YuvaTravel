using System;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.Guides
{
    public class GuideFaqCreateOrEdit
    {
        [Display(Name = "ID")]
        public Guid? GuideFaqId { get; set; }

        [Required(ErrorMessage = StrText.RequireSelectd)]
        [Display(Name = "嚮導")]
        public Guid? GuideId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "問題")]
        public string Question { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "回覆")]
        public string Answer { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
