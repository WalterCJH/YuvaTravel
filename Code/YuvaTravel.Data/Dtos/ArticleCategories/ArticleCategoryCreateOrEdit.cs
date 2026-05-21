using YuvaTravel.Base.Constants;
using YuvaTravel.Base.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.ArticleCategories
{
    public class ArticleCategoryCreateOrEdit
    {
        [Display(Name = "ID")]
        public Guid? ArticleCategoryId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(10, ErrorMessage = StrText.OverLength)]
        [Display(Name = "代碼")]
        [EngNumMix]
        public string Code { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(20, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "英文名稱")]
        public string NameEn { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "Url(絕對位置 或 相對位置)")]
        public string Url { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
