using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class Subscriber
    {
        [Key]
        [Display(Name = "ID")]
        public Guid SubscriberId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [MaxLength(254)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [Display(Name = "取消訂閱 Token")]
        public Guid UnsubscribeToken { get; set; }

        [Display(Name = "訂閱時間")]
        public DateTime SubscribeTime { get; set; }

        [Display(Name = "取消訂閱時間")]
        public DateTime? UnsubscribedTime { get; set; }

        [Display(Name = "確認時間 (double opt-in)")]
        public DateTime? ConfirmedTime { get; set; }

        // ---- 來源頁面 ----
        [Column(TypeName = "VARCHAR")]
        [MaxLength(500)]
        [Display(Name = "訂閱來源頁面 (相對路徑)")]
        public string SourcePath { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(1000)]
        [Display(Name = "訂閱來源完整 URL")]
        public string SourceUrl { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(1000)]
        [Display(Name = "Referrer (上一個來源)")]
        public string Referrer { get; set; }

        // ---- 訪客環境 ----
        [Column(TypeName = "VARCHAR")]
        [MaxLength(45)]
        [Display(Name = "IP 位址")]
        public string IpAddress { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(500)]
        [Display(Name = "User Agent")]
        public string UserAgent { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        [Display(Name = "Accept-Language")]
        public string AcceptLanguage { get; set; }

        // ---- 行銷活動 (UTM) ----
        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        [Display(Name = "UTM Source")]
        public string UtmSource { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        [Display(Name = "UTM Medium")]
        public string UtmMedium { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        [Display(Name = "UTM Campaign")]
        public string UtmCampaign { get; set; }

        public UserLog UserLog { get; set; }

        public Subscriber()
        {
            UserLog = new UserLog();
        }
    }
}
