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
    public class WebBanner
    {
        [Key]
        [Required]
        public Guid WebBannerId { get; set; }

        [Display(Name = "開始時間")]
        public DateTime BeginTime { get; set; }

        [Display(Name = "結束時間")]
        public DateTime EndTime { get; set; }

        [Display(Name = "期間")]
        public string RangeTime { get { return $"{BeginTime:yyyy/MM/dd} ~ {EndTime:yyyy/MM/dd}"; } }

        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "電腦圖片(W:1000、H:415)")]
        public string ImageUrl { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "手機圖片(W:660、H:660)")]
        public string ImageMobileUrl { get; set; }

        [MaxLength(50)]
        [Display(Name = "圖片標題(title)")]
        public string ImageTitle { get; set; }

        [MaxLength(100)]
        [Display(Name = "圖片說明(alt)")]
        public string ImageAlt { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "圖片點擊Url")]
        public string ImageClickUrl { get; set; }

        [Required]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public WebBanner()
        {
            UserLog = new UserLog();
        }

    }
}
