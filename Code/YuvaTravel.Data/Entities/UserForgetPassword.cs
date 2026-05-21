using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Entities
{
    public class UserForgetPassword
    {
        [Key]
        [Required]
        public Guid ForgetId { get; set; }

        public Guid UserGuid { get; set; }
        [ForeignKey("UserGuid")]
        public virtual User User { get; set; }

        public DateTime RequestTime { get; set; }

        public DateTime? CheckTime { get; set; }

    }
}
