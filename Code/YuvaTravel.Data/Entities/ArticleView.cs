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
    public class ArticleView
    {
        [Key]
        [Display(Name = "ID")]
        public Guid ArticleViewId { get; set; }

        public Guid ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

        public string IpAddress { get; set; }

        public DateTime CreateTime { get; set; }


        public ArticleView()
        {
        }

    }
}
