using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.QuestionAnswers
{
    public class QuestionAnswerFilter
    {
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [RegularExpression(@"(Title|DisplaySeq)")]
        public string SortBy { get; set; } = "DisplaySeq";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;
    }
}
