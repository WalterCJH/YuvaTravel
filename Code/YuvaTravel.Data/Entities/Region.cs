using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class Region
    {
        [Key]
        [Display(Name = "ID")]
        public Guid RegionId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(30)]
        [Display(Name = "代碼")]
        public string Code { get; set; }

        [MaxLength(50)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(80)]
        [Display(Name = "英文名稱")]
        public string NameEn { get; set; }

        [MaxLength(500)]
        [Display(Name = "說明")]
        public string Description { get; set; }

        [MaxLength(500)]
        [Display(Name = "圖片 (200×200)")]
        public string ImageUrl { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<GuideRegion> GuideRegions { get; set; }
        public virtual ICollection<ArticleRegion> ArticleRegions { get; set; }

        public Region()
        {
            UserLog = new UserLog();
        }
    }
}
