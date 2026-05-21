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
    public class UrlReferrer
    {
        [Key]
        [Display(Name = "ID")]
        public Guid UrlReferrerId { get; set; }

        [Display(Name = "來源代碼")]
        public Guid? UrlReferrerCodeId { get; set; }
        [ForeignKey("UrlReferrerCodeId")]
        public virtual UrlReferrerCode UrlReferrerCode { get; set; }

        [MaxLength(100)]
        [Display(Name = "來源代碼")]
        public string ReferrerCode { get; set; }

        [MaxLength(2000)]
        [Display(Name = "來源網址")]
        public string SourceUrl { get; set; }

        [MaxLength(2000)]
        [Display(Name = "目的網址")]
        public string DestinationUrl { get; set; }

        [MaxLength(100)]
        [Display(Name = "IP位置")]
        public string IpAddress { get; set; }

        [MaxLength(100)]
        [Display(Name = "CloudFlare IP位置")]
        public string CloudFlareIpAddress { get; set; }

        [Display(Name = "是否為手機裝置")]
        public bool IsMobile { get; set; }

        [MaxLength(1000)]
        [Display(Name = "系統名稱")]
        public string OS { get; set; }

        [MaxLength(2000)]
        [Display(Name = "使用者資訊")]
        public string UserAgent { get; set; }

        [MaxLength(1000)]
        [Display(Name = "使用者系統")]
        public string Platform { get; set; }

        [MaxLength(100)]
        [Display(Name = "瀏覽器名稱")]
        public string BrowserType { get; set; }

        [MaxLength(100)]
        [Display(Name = "瀏覽器版本")]
        public string BrowserVersion { get; set; }

        [Display(Name = "IsOnActionExecuting")]
        public bool IsOnActionExecuting { get; set; }

        [Display(Name = "IsOnActionExecuted")]
        public bool IsOnActionExecuted { get; set; }

        [Display(Name = "後台紀錄")]
        public bool IsAdmin { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }


        public UrlReferrer()
        {
        }

    }
}
