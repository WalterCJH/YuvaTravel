using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class Guide
    {
        [Key]
        [Display(Name = "ID")]
        public Guid GuideId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [Display(Name = "本月精選")]
        public bool IsFeatured { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(50)]
        [Display(Name = "代碼")]
        public string Code { get; set; }

        [NotMapped]
        public string GetCodeOrId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code))
                    return Code;
                return GuideId.ToString();
            }
        }

        [MaxLength(50)]
        [Display(Name = "中文名")]
        public string Name { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(80)]
        [Display(Name = "英文名")]
        public string NameEn { get; set; }

        [Display(Name = "在地年數")]
        public int YearsOfExperience { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(50)]
        [Display(Name = "可用語言 (例 JP·EN)")]
        public string Languages { get; set; }

        [MaxLength(200)]
        [Display(Name = "短簡介")]
        public string Bio { get; set; }

        [Display(Name = "完整介紹")]
        public string LongBio { get; set; }

        [MaxLength(300)]
        [Display(Name = "個人語錄")]
        public string QuoteText { get; set; }

        [MaxLength(50)]
        [Display(Name = "語錄署名")]
        public string QuoteBy { get; set; }

        [MaxLength(500)]
        [Display(Name = "肖像圖")]
        public string PortraitUrl { get; set; }

        [MaxLength(500)]
        [Display(Name = "封面圖")]
        public string CoverUrl { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        [Display(Name = "評分")]
        public decimal Rating { get; set; }

        [Display(Name = "評論數")]
        public int ReviewCount { get; set; }

        [MaxLength(50)]
        [Display(Name = "服務形式")]
        public string ServiceFormat { get; set; }

        [Display(Name = "適合人數下限")]
        public int? MinPeople { get; set; }

        [Display(Name = "適合人數上限")]
        public int? MaxPeople { get; set; }

        [MaxLength(50)]
        [Display(Name = "回覆時間")]
        public string ResponseTime { get; set; }

        [Display(Name = "起價 (NT$)")]
        public int? StartingPrice { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<GuideTag> GuideTags { get; set; }
        public virtual ICollection<GuideRoute> GuideRoutes { get; set; }
        public virtual ICollection<GuideReview> GuideReviews { get; set; }
        public virtual ICollection<GuideFaq> GuideFaqs { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
        public virtual ICollection<GuideRegion> GuideRegions { get; set; }

        public Guide()
        {
            UserLog = new UserLog();
        }
    }
}
