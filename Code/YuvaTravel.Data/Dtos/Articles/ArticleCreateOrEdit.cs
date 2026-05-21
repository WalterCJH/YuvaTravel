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
using YuvaTravel.Data.Dtos.ArticleComments;
using YuvaTravel.Data.Dtos.Regions;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Data.Dtos.Articles
{

    public class ArticleCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork))!;
            if (uow.ArticleRepo.IsTitleRepeat(Title, ArticleId))
            {
                yield return new ValidationResult(StrText.TitleNotRepeat, new string[] { nameof(Title) });
            }
            if (!string.IsNullOrWhiteSpace(Code) && uow.ArticleRepo.IsCodeRepeat(Code, ArticleId))
            {
                yield return new ValidationResult(StrText.CodeNotRepeat, new string[] { nameof(Code) });
            }
            if (!string.IsNullOrWhiteSpace(MetaTitle) && MetaTitle.Contains("\""))
            {
                yield return new ValidationResult(StrText.NoInputDoubleQuotes, new string[] { nameof(MetaTitle) });
            }
            if (!string.IsNullOrWhiteSpace(MetaDescription) && MetaDescription.Contains("\""))
            {
                yield return new ValidationResult(StrText.NoInputDoubleQuotes, new string[] { nameof(MetaDescription) });
            }
            if (ArticleType == YuvaTravel.Base.Enum.ArticleType.活動)
            {
                if (string.IsNullOrWhiteSpace(EventLocation))
                {
                    yield return new ValidationResult(StrText.RequiredNotName, new string[] { nameof(EventLocation) });
                }
                if (EventStarNum == null)
                {
                    yield return new ValidationResult(StrText.RequiredNotName, new string[] { nameof(EventStarNum) });
                }
                if (string.IsNullOrWhiteSpace(EventCount))
                {
                    yield return new ValidationResult(StrText.RequiredNotName, new string[] { nameof(EventCount) });
                }
                if (EventPrice == null)
                {
                    yield return new ValidationResult(StrText.RequiredNotName, new string[] { nameof(EventPrice) });
                }
            }
        }

        [Display(Name = "ID")]
        public Guid? ArticleId { get; set; }

        //[Display(Name = "文章類別")]
        //public Guid? ArticleCategoryId { get; set; }


        [Display(Name = "全選")]
        public bool CheckAll { get; set; }

        public List<ArticleCategoryDto> ArticleCategories { get; set; } = new List<ArticleCategoryDto>();

        [Display(Name = "全選地區")]
        public bool CheckAllRegions { get; set; }

        public List<RegionPickDto> Regions { get; set; } = new List<RegionPickDto>();

        public void SettingRegions(List<Entities.Region> allRegions, List<Guid> activeRegionIdList = null)
        {
            foreach (var region in allRegions.OrderBy(p => p.DisplaySeq))
            {
                var pick = new RegionPickDto
                {
                    RegionId = region.RegionId,
                    Name = region.Name,
                    IsActive = activeRegionIdList != null && activeRegionIdList.Contains(region.RegionId)
                };
                Regions.Add(pick);
            }
        }


        [Required(ErrorMessage = StrText.RequireSelectd)]
        [Display(Name = "文章類型")]
        public ArticleType? ArticleType { get; set; }

        [Display(Name = "置頂")]
        public bool IsTop { get; set; }

        [Display(Name = "精選文章")]
        public bool IsFeaturedArticle { get; set; }

        [Display(Name = "精選順序")]
        public int FeaturedArticleDisplaySeq { get; set; }

        [Display(Name = "文章作者")]
        public Guid? AuthorId { get; set; }

        [Display(Name = "閱讀時間 (分鐘)")]
        public int? ReadTimeMin { get; set; }

        [Display(Name = "文章上線時間")]
        public DateTime? OnlineTime { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "推廣碼")]
        public string PromotionCode { get; set; }

        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "文章代碼 (自定義網址最後的文字)")]
        public string Code { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "文章標題")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "文章描述")]
        public string Description { get; set; }
                
        [MaxLength(32000, ErrorMessage = StrText.OverLength)]
        [Display(Name = "文章內容")]
        public string Content { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "Meta標題")]
        public string MetaTitle { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "Meta敘述")]
        public string MetaDescription { get; set; }

        [Display(Name = "圖片檔案")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "圖片檔案Base64")]
        public string ImageFileBase64 { get; set; }

        [Display(Name = "圖片檔案名稱")]
        public string ImageFileName { get; set; }

        [Display(Name = "圖片檔案類型")]
        public string ImageFileType { get; set; }

        [Display(Name = "文章上線排程時間")]
        public DateTime? OnScheduleTime { get; set; }


        [Display(Name = "Banner手機圖片檔案")]
        public IFormFile ImageMobileFile { get; set; }

        [Display(Name = "Banner手機圖片檔案Base64")]
        public string ImageMobileFileBase64 { get; set; }

        [Display(Name = "Banner手機圖片檔案名稱")]
        public string ImageMobileFileName { get; set; }

        [Display(Name = "Banner手機圖片檔案類型")]
        public string ImageMobileFileType { get; set; }


        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "圖片標題(title)")]
        public string ImageTitle { get; set; }

        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "圖片說明(alt)")]
        public string ImageAlt { get; set; }

        [Display(Name = "圖片路徑")]
        public string ImagePath_ { get; set; }

        [Display(Name = "Banner手機圖片路徑")]
        public string ImageBannerMobilePath_ { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public string[] TagIds { get; set; }

        [Display(Name = "未回覆")]
        public int NotReplyCount { get; set; }


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

        public List<ArticleSDCommonQuestionCreateOrEdit> ArticleSDCommonQuestions { get; set; }

        public ArticleCreateOrEdit()
        {
            ArticleComments = new List<ArticleCommentCreateOrEdit>();
            ArticleComment = new ArticleCommentCreateOrEdit();
            ArticleSDCommonQuestions = new List<ArticleSDCommonQuestionCreateOrEdit>();
        }

        public void SettingArticleCategory(List<ArticleCategory> allArticleCategories, List<Guid> activeArticleCategoryIdList = null)
        {
            foreach (var articleCategory in allArticleCategories.OrderBy(p => p.DisplaySeq))
            {
                var articleCategoryDto = new ArticleCategoryDto();
                ArticleCategories.Add(articleCategoryDto);
                articleCategoryDto.ArticleCategoryId = articleCategory.ArticleCategoryId;
                articleCategoryDto.Name = articleCategory.Name;
                if (activeArticleCategoryIdList != null && activeArticleCategoryIdList.Contains(articleCategory.ArticleCategoryId))
                {
                    articleCategoryDto.IsActive = true;
                }
            }
        }
    }

}
