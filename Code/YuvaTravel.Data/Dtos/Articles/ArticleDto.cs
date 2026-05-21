using System;

namespace YuvaTravel.Data.Dtos.Articles
{

    public class ArticleDto
    {
        public Guid ArticleId { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public int DisplaySeq { get; set; }
    }
}
