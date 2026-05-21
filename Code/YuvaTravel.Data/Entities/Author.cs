using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class Author
    {
        [Key]
        [Display(Name = "ID")]
        public Guid AuthorId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; } = true;

        [Required]
        [MaxLength(50)]
        [Display(Name = "作者名稱")]
        public string Name { get; set; }

        [MaxLength(80)]
        [Display(Name = "英文名")]
        public string NameEn { get; set; }

        [MaxLength(500)]
        [Display(Name = "簡介")]
        public string Bio { get; set; }

        [MaxLength(500)]
        [Display(Name = "肖像圖")]
        public string AvatarUrl { get; set; }

        [Display(Name = "關聯嚮導")]
        public Guid? GuideId { get; set; }

        [ForeignKey("GuideId")]
        public virtual Guide Guide { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        public Author()
        {
            UserLog = new UserLog();
        }
    }
}
