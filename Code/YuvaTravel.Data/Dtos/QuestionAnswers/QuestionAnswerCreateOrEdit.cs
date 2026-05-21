using YuvaTravel.Base.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.QuestionAnswers
{
    public class QuestionAnswerCreateOrEdit
    {
        [Display(Name = "ID")]
        public Guid? QuestionAnswerId { get; set; }

        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "Url(絕對位置 或 相對位置)")]
        public string Url { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "標題")]
        public string Title { get; set; }
                
        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(4000, ErrorMessage = StrText.OverLength)]
        [Display(Name = "內容")]
        public string Content { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "顯示順序")]
        public int DisplaySeq { get; set; }
    }
}
