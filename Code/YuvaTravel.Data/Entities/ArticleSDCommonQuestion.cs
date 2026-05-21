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
    public class ArticleSDCommonQuestion
    {
        [Key]
        [Display(Name = "ID")]
        public Guid ArticleSDCommonQuestionId { get; set; }

        public Guid ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

        [Display(Name = "顯示順序")]
        public int DisplaySequence { get; set; }

        [MaxLength(200)]
        [Display(Name = "留言內容")]
        public string Name { get; set; }

        [MaxLength(4000)]
        [Display(Name = "留言人員")]
        public string Text { get; set; }

    }
}
