using YuvaTravel.Data.Columns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YuvaTravel.Data.Entities
{
    public class ArticleCategoryArticle
    {
        [Key]
        [Required]
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        [Display(Name = "ArticleCategoryId")]
        public Guid ArticleCategoryId { get; set; }
        [ForeignKey("ArticleCategoryId")]
        public virtual ArticleCategory ArticleCategory { get; set; }

        [Display(Name = "ArticleId")]
        public Guid ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

        public UserLog UserLog { get; set; }

        public ArticleCategoryArticle()
        {
            UserLog = new UserLog();
        }
    }
}
