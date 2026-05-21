using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.Authors
{
    public class AuthorCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 沒選嚮導時才必填名稱;選了嚮導會由 Controller 從 Guide 帶資料
            if (!GuideId.HasValue && string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult(StrText.Required, new[] { nameof(Name) });
            }
        }

        [Display(Name = "ID")]
        public Guid? AuthorId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; } = true;

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "作者名稱")]
        public string Name { get; set; }

        [MaxLength(80, ErrorMessage = StrText.OverLength)]
        [Display(Name = "英文名")]
        public string NameEn { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "簡介")]
        public string Bio { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "肖像圖 URL")]
        public string AvatarUrl { get; set; }

        [Display(Name = "肖像圖檔案 (200×200)")]
        public IFormFile AvatarFile { get; set; }

        [Display(Name = "目前肖像")]
        public string AvatarUrl_ { get; set; }

        [Display(Name = "關聯嚮導 (可不選)")]
        public Guid? GuideId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
