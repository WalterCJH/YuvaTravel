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
    public class UrlReferrerCode
    {
        [Key]
        [Display(Name = "ID")]
        public Guid UrlReferrerCodeId { get; set; }

        [MaxLength(20)]
        [Display(Name = "代碼")]
        public string Code { get; set; }

        [MaxLength(100)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [MaxLength(200)]
        [Display(Name = "敘述")]
        public string Description { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<UrlReferrer> UrlReferrers { get; set; }

        public UrlReferrerCode()
        {
            UserLog = new UserLog();
        }

    }
}
