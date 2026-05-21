using YuvaTravel.Data.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Entities
{
    public class WebMeta
    {
        [Key]
        [Required]
        public Guid WebMetaId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "網址")]
        public string Url { get; set; }

        [Required]
        [MaxLength(20)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "og:type")]
        public string OgType { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "網頁標題(title、og:title)")]
        public string MetaTitle { get; set; }

        [MaxLength(1000)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "網頁說明(Description、og:Description)")]
        public string MetaDescription { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "網頁圖片(og:image)")]
        public string MetaImageUrl { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "Canonical")]
        public string Canonical { get; set; }

        [Display(Name = "Canonical取得該頁面網址絕對路徑")]
        public bool IsCanonicalAbsoluteUri { get; set; }

        [Required]
        [Display(Name = "顯示順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public WebMeta()
        {
            UserLog = new UserLog();
        }

    }
}
