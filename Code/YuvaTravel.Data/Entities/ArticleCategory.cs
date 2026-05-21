using YuvaTravel.Data.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YuvaTravel.Data.Entities
{
    public class ArticleCategory
    {
        [Key]
        [Display(Name = "ID")]
        public Guid ArticleCategoryId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(200)]
        [Display(Name = "Url(絕對位置 或 相對位置)")]
        public string Url { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(10)]
        [Display(Name = "代碼")]
        public string Code { get; set; }

        [MaxLength(20)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(30)]
        [Display(Name = "英文名稱")]
        public string NameEn { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<ArticleCategoryArticle> ArticleCategoryArticles { get; set; }

        public ArticleCategory()
        {
            UserLog = new UserLog();
        }
    }
}
