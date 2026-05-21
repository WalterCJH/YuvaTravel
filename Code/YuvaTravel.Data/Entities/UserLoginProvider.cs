using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YuvaTravel.Data.Entities
{
    [PrimaryKey(nameof(LoginProvider), nameof(ProviderKey), nameof(UserGuid))]
    public class UserLoginProvider
    {
        [Required]
        [MaxLength(128)]
        [Display(Name = "外部登入系統商")]
        public string LoginProvider { get; set; }

        [Required]
        [MaxLength(128)]
        [Display(Name = "外部登入金鑰")]
        public string ProviderKey { get; set; }

        [Display(Name = "使用者")]
        public Guid UserGuid { get; set; }
        [ForeignKey("UserGuid")]
        public virtual User User { get; set; }

        [Display(Name = "新增時間")]
        public DateTime CreateTime { get; set; }

    }
}
