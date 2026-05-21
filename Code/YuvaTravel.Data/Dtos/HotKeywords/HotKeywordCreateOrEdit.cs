using YuvaTravel.Base.Constants;
using YuvaTravel.Base.ValidationAttributes;
using YuvaTravel.Data.Uow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.HotKeywords
{
    public class HotKeywordCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow != null && uow.HotKeywordRepo.IsNameRepeat(Name, HotKeywordId))
            {
                yield return new ValidationResult(StrText.NameNotRepeat, new[] { nameof(Name) });
            }
        }

        [Display(Name = "ID")]
        public Guid? HotKeywordId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(20, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
