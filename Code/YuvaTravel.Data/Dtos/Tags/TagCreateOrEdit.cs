using YuvaTravel.Base.Constants;
using YuvaTravel.Base.ValidationAttributes;
using YuvaTravel.Data.Uow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Tags
{
    public class TagCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow == null) yield break;

            if (uow.TagRepo.IsNameRepeat(Name, TagId))
            {
                yield return new ValidationResult(StrText.NameNotRepeat, new[] { nameof(Name) });
            }
        }

        [Display(Name = "ID")]
        public Guid? TagId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(20, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "敘述")]
        public string Description { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }
    }
}
