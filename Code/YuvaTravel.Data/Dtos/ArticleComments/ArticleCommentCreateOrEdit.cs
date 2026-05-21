using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YuvaTravel.Data.Dtos.ArticleComments
{

    public class ArticleCommentCreateOrEdit
    {

        [Display(Name = "ID")]
        public Guid? ArticleCommentId { get; set; }

        [Display(Name = "文章")]
        public Guid ArticleId { get; set; }


        [MaxLength(1000, ErrorMessage = StrText.OverLength)]
        [Display(Name = "留言內容")]
        public string CommentContent { get; set; }

        [Display(Name = "留言人員")]
        public string CommentUserId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "留言時間")]
        public DateTime? CommentTime { get; set; }


        [MaxLength(1000, ErrorMessage = StrText.OverLength)]
        [Display(Name = "回覆內容")]
        public string ReplyContent { get; set; }

        [Display(Name = "回覆人員")]
        public string ReplyUserId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "回覆時間")]
        public DateTime? ReplyTime { get; set; }


        [Display(Name = "目前使用者")]
        public string UserId { get; set; }

        public bool IsEditOrDelete
        {
            get
            {
                if (CommentUserId != UserId)
                    return false;

                if (ReplyTime != null)
                    return false;

                return true;
            }
        }

        public bool IsReply
        {
            get
            {
                if (ReplyUserId != null && ReplyUserId != UserId)
                    return false;

                if (ReplyTime != null && ReplyTime.Value.AddDays(1) < DateTime.Now)
                    return false;

                return true;
            }
        }


        public ArticleCommentCreateOrEdit()
        {
        }
    }

}
