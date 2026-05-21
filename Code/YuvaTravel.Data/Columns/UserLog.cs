using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YuvaTravel.Data.Columns
{
    [Owned]
    public class UserLog
    {
        [Column("CreateUserId", TypeName = "VARCHAR")]
        [MaxLength(30)]
        [Display(Name = "新增使用者")]
        public string CreateUserId { get; set; }

        [Column("CreateTime")]
        [Display(Name = "新增時間")]
        public DateTime? CreateTime { get; set; }

        [Column("UpdateUserId", TypeName = "VARCHAR")]
        [MaxLength(30)]
        [Display(Name = "修改使用者")]
        public string UpdateUserId { get; set; }

        [Column("UpdateTime")]
        [Display(Name = "修改時間")]
        public DateTime? UpdateTime { get; set; }
    }
}
