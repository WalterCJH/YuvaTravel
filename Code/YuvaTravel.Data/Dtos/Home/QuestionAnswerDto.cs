using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Home
{
    public class QuestionAnswerDto
    {
        [Display(Name = "Url(絕對位置 或 相對位置)")]
        public string Url { get; set; }

        [Display(Name = "標題")]
        public string Title { get; set; }

        [Display(Name = "內容")]
        public string Content { get; set; }

        [Display(Name = "顯示順序")]
        public int DisplaySeq { get; set; }

        [Display(Name = "標題數字")]
        public string ShowNumber { get; set; }
    }
}
