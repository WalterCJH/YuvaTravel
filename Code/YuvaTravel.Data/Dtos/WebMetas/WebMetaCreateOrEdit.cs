using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Uow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace YuvaTravel.Data.Dtos.WebMetas
{

    public class WebMetaCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow == null) yield break;

            if (uow.WebMetaRepo.IsUrlRepeat(Url, WebMetaId))
            {
                yield return new ValidationResult(StrText.UrlNotRepeat, new[] { nameof(Url) });
            }
        }

        [Display(Name = "ID")]
        public Guid? WebMetaId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "網址")]
        public string Url { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "og:type")]
        public string OgType { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "網頁標題(title、og:title)")]
        public string MetaTitle { get; set; }

        [MaxLength(1000, ErrorMessage = StrText.OverLength)]
        [Display(Name = "網頁說明(Description、og:Description)")]
        public string MetaDescription { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        [Display(Name = "圖片檔案(og:image)")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "圖片路徑")]
        public string MetaImageUrl_ { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "Canonical")]
        public string Canonical { get; set; }

        [Display(Name = "Canonical取得該頁面網址絕對路徑")]
        public bool IsCanonicalAbsoluteUri { get; set; }

        public WebMetaCreateOrEdit()
        {
        }
    }

}
