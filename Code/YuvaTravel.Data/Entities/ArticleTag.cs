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
    public class ArticleTag
    {
        [Key]
        [Display(Name = "ID")]
        public Guid ArticleTagId { get; set; }

        public Guid TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        public Guid ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

    }
}
