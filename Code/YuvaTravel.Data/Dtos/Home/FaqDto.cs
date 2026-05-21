using System.Collections.Generic;

namespace YuvaTravel.Data.Dtos.Home
{
    public class FaqDto
    {
        public List<QuestionAnswerDto> QuestionAnswers { get; set; } = new List<QuestionAnswerDto>();

        public AsideSearchTagDto AsideSearchTagDto { get; set; } = new AsideSearchTagDto();
    }
}
