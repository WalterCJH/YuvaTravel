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
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.ArticleComments;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Data.Dtos.ArticleReviews
{

    public class ArticleReviewEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork))!;
            if (ArticleReviewType == ArticleReviewType.已通過 && !uow.ArticleRepo.IsAllReply(ArticleId))
            {
                yield return new ValidationResult("還有文章尚未回覆，無法通過審核", new string[] { nameof(ArticleReviewType) });
            }
            else if (ArticleReviewType == ArticleReviewType.排程中 && OnScheduleTime == null)
            {
                yield return new ValidationResult("審核狀態為排程中，文章上線排程時間必須輸入", new string[] { nameof(OnScheduleTime) });
            }
        }

        [Display(Name = "ID")]
        public Guid ArticleId { get; set; }

        [Display(Name = "文章類別")]
        public List<ArticleCategoryArticle> ArticleCategoryArticles { get; set; }

        [Display(Name = "審核狀態")]
        public ArticleReviewType ArticleReviewType { get; set; }

        [Display(Name = "文章上線排程時間")]
        public DateTime? OnScheduleTime { get; set; }

        [Display(Name = "置頂")]
        public bool IsTop { get; set; }

        [Display(Name = "精選文章")]
        public bool IsFeaturedArticle { get; set; }

        [Display(Name = "精選文章順序")]
        public int FeaturedArticleDisplaySeq { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "推廣碼")]
        public string PromotionCode { get; set; }

        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "文章代碼 (自定義網址最後的文字)")]
        public string Code { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "文章標題")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "文章描述")]
        public string Description { get; set; }

        [MaxLength(50)]
        [Display(Name = "Meta標題")]
        public string MetaTitle { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "Meta敘述")]
        public string MetaDescription { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "圖片標題(title)")]
        public string ImageTitle { get; set; }

        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "圖片說明(alt)")]
        public string ImageAlt { get; set; }

        [Display(Name = "圖片路徑")]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public string[] TagIds { get; set; }

        [Display(Name = "未回覆")]
        public int NotReplyCount { get; set; }

        public string CommentClass
        {
            get
            {
                if (NotReplyCount == 0)
                {
                    return "collapsed-card";
                }
                else
                {
                    return "";
                }
            }
        }
        public string CommentIconClass
        {
            get
            {
                if (NotReplyCount == 0)
                {
                    return "fas fa-plus";
                }
                else
                {
                    return "fas fa-minus";
                }
            }
        }

        public List<ArticleCommentCreateOrEdit> ArticleComments { get; set; }

        public ArticleCommentCreateOrEdit ArticleComment { get; set; }


        public ArticleReviewEdit()
        {
            ArticleComments = new List<ArticleCommentCreateOrEdit>();
            ArticleComment = new ArticleCommentCreateOrEdit();
        }
    }

}
