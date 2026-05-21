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
    public class RecordClick
    {
        [Key]
        [Required]
        public Guid RecordClickId { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "IP位置")]
        public string IpAddress { get; set; }

        [MaxLength(1000)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "點擊網址")]
        public string ClickUrl { get; set; }

        [Display(Name = "點擊時間")]
        public DateTime ClickTime { get; set; }

        [MaxLength(1000)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "來源網址")]
        public string UrlReferrer { get; set; }

        [MaxLength(200)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "點擊備註")]
        public string ClickMemo { get; set; }

        [Display(Name = "平板、手機裝置")]
        public bool IsMobile { get; set; }

    }
}
