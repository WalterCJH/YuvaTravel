using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using YuvaTravel.Data.Columns;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Entities
{
    public class WebConfig
    {
        [Key]
        [Required]
        public Guid WebConfigId { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        [Display(Name = "前往投注")]
        public string GoRegister { get; set; }

        public UserLog UserLog { get; set; }

        public WebConfig()
        {
            UserLog = new UserLog();
        }

    }
}
