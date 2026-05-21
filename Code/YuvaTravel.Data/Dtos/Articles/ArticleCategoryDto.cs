using System;

namespace YuvaTravel.Data.Dtos.Articles
{
    public class ArticleCategoryDto
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public Guid ArticleCategoryId { get; set; }
    }
}
