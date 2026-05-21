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
    public class ArticleComment
    {
        [Key]
        [Display(Name = "ID")]
        public Guid ArticleCommentId { get; set; }

        public Guid ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(1000)]
        [Display(Name = "留言內容")]
        public string CommentContent { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(30)]
        [Display(Name = "留言人員")]
        public string CommentUserId { get; set; }

        [Display(Name = "留言時間")]
        public DateTime CommentTime { get; set; }


        [Column(TypeName = "NVARCHAR")]
        [MaxLength(1000)]
        [Display(Name = "回覆內容")]
        public string ReplyContent { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(30)]
        [Display(Name = "回覆人員")]
        public string ReplyUserId { get; set; }

        [Display(Name = "回覆時間")]
        public DateTime? ReplyTime { get; set; }


        public ArticleComment()
        {
        }

    }
}
