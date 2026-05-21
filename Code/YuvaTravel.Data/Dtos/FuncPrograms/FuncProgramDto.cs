using YuvaTravel.Base.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Enum;

namespace YuvaTravel.Data.Dtos.FuncPrograms
{
    public class FuncProgramDto : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsActiveCreate == false && IsChangeCreate)
            {
                yield return new ValidationResult($"啟用新增未勾選，變更新增不能勾選", new string[] { nameof(IsChangeCreate) });
            }
            if (IsActiveEdit == false && IsChangeEdit)
            {
                yield return new ValidationResult($"啟用修改未勾選，變更修改不能勾選", new string[] { nameof(IsChangeEdit) });
            }
            if (IsActiveDelete == false && IsChangeDelete)
            {
                yield return new ValidationResult($"啟用刪除未勾選，變更刪除不能勾選", new string[] { nameof(IsChangeDelete) });
            }
            if (IsActiveDetails == false && IsChangeDetails)
            {
                yield return new ValidationResult($"啟用檢視未勾選，變更檢視不能勾選", new string[] { nameof(IsChangeDetails) });
            }
            if (IsActiveImport == false && IsChangeImport)
            {
                yield return new ValidationResult($"啟用匯入未勾選，變更匯入不能勾選", new string[] { nameof(IsChangeImport) });
            }
            if (IsActiveExport == false && IsChangeExport)
            {
                yield return new ValidationResult($"啟用匯出未勾選，變更匯出不能勾選", new string[] { nameof(IsChangeExport) });
            }

        }

        [Required]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "網址")]
        public string Url { get; set; }

        [Display(Name = "啟用新增")]
        public bool IsActiveCreate { get; set; }

        [Display(Name = "啟用修改")]
        public bool IsActiveEdit { get; set; }

        [Display(Name = "啟用刪除")]
        public bool IsActiveDelete { get; set; }

        [Display(Name = "啟用檢視")]
        public bool IsActiveDetails { get; set; }

        [Display(Name = "啟用匯入")]
        public bool IsActiveImport { get; set; }

        [Display(Name = "啟用匯出")]
        public bool IsActiveExport { get; set; }

        [Display(Name = "變更新增")]
        public bool IsChangeCreate { get; set; }

        [Display(Name = "變更修改")]
        public bool IsChangeEdit { get; set; }


        [Display(Name = "變更刪除")]
        public bool IsChangeDelete { get; set; }

        [Display(Name = "變更檢視")]
        public bool IsChangeDetails { get; set; }

        [Display(Name = "變更匯入")]
        public bool IsChangeImport { get; set; }

        [Display(Name = "變更匯出")]
        public bool IsChangeExport { get; set; }


        [Required]
        [Display(Name = "顯示順序")]
        public int DisplaySeq { get; set; }

        [Required(ErrorMessage = StrText.RequireSelectd)]
        [Display(Name = "權限等級")]
        public AuthorizeLevel AuthorizeLevel { get; set; }

        [Display(Name = "權限等級")]
        public AuthorizeLevel AuthorizeLevelView { get { return AuthorizeLevel; } }

        public FuncProgramDto()
        {
        }
    }
}
