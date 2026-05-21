using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Data.Dtos.Regions
{
    public class RegionCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow != null && !string.IsNullOrWhiteSpace(Code) && uow.RegionRepo.IsCodeRepeat(Code, RegionId))
            {
                yield return new ValidationResult(StrText.CodeNotRepeat, new[] { nameof(Code) });
            }
        }

        [Display(Name = "ID")]
        public Guid? RegionId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "代碼")]
        public string Code { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [MaxLength(80, ErrorMessage = StrText.OverLength)]
        [Display(Name = "英文名稱")]
        public string NameEn { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "說明")]
        public string Description { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "圖片 URL")]
        public string ImageUrl { get; set; }

        [Display(Name = "圖片檔案 (200×200)")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "目前圖片")]
        public string ImageUrl_ { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
