using YuvaTravel.Data.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Data.Entities
{
    public class Article
    {
        [Key]
        [Display(Name = "ID")]
        public Guid ArticleId { get; set; }

        [Display(Name = "文章類型")]
        public ArticleType ArticleType { get; set; }

        [Display(Name = "審核狀態")]
        public ArticleReviewType ArticleReviewType { get; set; }

        public string ArticleReviewTypeClass
        {
            get
            {
                if (ArticleReviewType == ArticleReviewType.已通過)
                    return "text-success";
                else if (ArticleReviewType == ArticleReviewType.未通過)
                    return "text-danger";
                else if (ArticleReviewType == ArticleReviewType.排程中)
                    return "text-info";
                else
                    return "text-secondary";
            }
        }

        [Display(Name = "置頂")]
        public bool IsTop { get; set; }

        [Display(Name = "精選文章")]
        public bool IsFeaturedArticle { get; set; }

        [Display(Name = "精選順序")]
        public int FeaturedArticleDisplaySeq { get; set; }

        [MaxLength(50)]
        [Display(Name = "推廣碼")]
        public string PromotionCode { get; set; }

        [Display(Name = "文章作者")]
        public Guid? AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }

        //[Display(Name = "文章類別")]
        //public Guid? ArticleCategoryId { get; set; }
        //[ForeignKey("ArticleCategoryId")]
        //public virtual ArticleCategory ArticleCategory { get; set; }

        [Display(Name = "文章序號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SerialNumber { get; set; }

        [MaxLength(50)]
        [Display(Name = "文章代碼")]
        public string Code { get; set; }

        [NotMapped]
        public string GetCodeOrId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code))
                    return Code;
                return ArticleId.ToString();
            }
        }

        [MaxLength(50)]
        [Display(Name = "文章標題")]
        public string Title { get; set; }

        [MaxLength(500)]
        [Display(Name = "文章描述")]
        public string Description { get; set; }

        [Display(Name = "文章內容")]
        public string Content { get; set; }

        [Display(Name = "文章內容2")]
        public string Content2 { get; set; }

        [Display(Name = "文章內容3")]
        public string Content3 { get; set; }

        [Display(Name = "文章內容4")]
        public string Content4 { get; set; }

        [NotMapped]
        public string ContentAll
        {
            get
            {
                return Content + Content2 + Content3 + Content4;
            }
        }

        [MaxLength(50)]
        [Display(Name = "Meta標題")]
        public string MetaTitle { get; set; }

        [MaxLength(500)]
        [Display(Name = "Meta敘述")]
        public string MetaDescription { get; set; }

        [MaxLength(500)]
        [Display(Name = "文章圖片路徑")]
        public string ImagePath { get; set; }

        [MaxLength(500)]
        [Display(Name = "文章首頁圖片路徑")]
        public string ImageHomePath { get; set; }

        [MaxLength(500)]
        [Display(Name = "文章類別圖片路徑")]
        public string ImageCategoryPath { get; set; }

        [MaxLength(500)]
        [Display(Name = "手機文章圖片路徑")]
        public string ImageMobilePath { get; set; }

        [MaxLength(500)]
        [Display(Name = "首頁Banner手機圖片路徑")]
        public string ImageBannerMobilePath { get; set; }

        [MaxLength(50)]
        [Display(Name = "圖片標題(title)")]
        public string ImageTitle { get; set; }

        [MaxLength(100)]
        [Display(Name = "圖片說明(alt)")]
        public string ImageAlt { get; set; }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        [Display(Name = "觀看次數")]
        public int Views { get; set; }

        [Display(Name = "閱讀時間(分)")]
        public int? ReadTimeMin { get; set; }

        [MaxLength(30)]
        [Display(Name = "文章審核人員")]
        public string ReviewUserId { get; set; }

        [Display(Name = "文章審核時間")]
        public DateTime? ReviewTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [Display(Name = "文章上線排程時間")]
        public DateTime? OnScheduleTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [Display(Name = "文章上線時間")]
        public DateTime? OnlineTime { get; set; }


        #region 精選活動

        [MaxLength(300)]
        [Display(Name = "國家")]
        public string EventLocation { get; set; }

        [Display(Name = "星星數")]
        public int? EventStarNum { get; set; }

        [MaxLength(20)]
        [Display(Name = "評論數")]
        public string EventCount { get; set; }

        [Display(Name = "價格")]
        public int? EventPrice { get; set; }

        [MaxLength(100)]
        [Display(Name = "優惠方式")]
        public string EventDiscount { get; set; }

        [MaxLength(100)]
        [Display(Name = "優惠折扣")]
        public string EventDiscountValue { get; set; }

        #endregion


        public UserLog UserLog { get; set; }

        public virtual ICollection<ArticleTag> ArticleTags { get; set; }

        public virtual ICollection<ArticleView> ArticleViews { get; set; }

        public virtual ICollection<ArticleComment> ArticleComments { get; set; }

        public virtual ICollection<ArticleSDCommonQuestion> ArticleSDCommonQuestions { get; set; }

        [Display(Name = "文章類別")]
        public virtual ICollection<ArticleCategoryArticle> ArticleCategoryArticles { get; set; }

        [Display(Name = "地區")]
        public virtual ICollection<ArticleRegion> ArticleRegions { get; set; }

        [NotMapped]
        public int NotReplyCount
        {
            get
            {
                if (ArticleComments == null)
                    return 0;

                return ArticleComments.Count(p => p.ReplyTime == null);
            }
        }

        [NotMapped]
        public string[] TagIds { get; set; }

        public Article()
        {
            UserLog = new UserLog();
        }

    }
}
