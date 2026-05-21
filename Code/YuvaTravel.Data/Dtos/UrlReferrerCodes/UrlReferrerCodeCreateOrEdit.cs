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
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Data.Dtos.UrlReferrerCodes
{

    public class UrlReferrerCodeCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork))!;
            if (uow.UrlReferrerCodeRepo.IsCodeRepeat(Code, UrlReferrerCodeId))
            {
                yield return new ValidationResult(StrText.CodeNotRepeat, new string[] { nameof(Code) });
            }
        }

        [Display(Name = "ID")]
        public Guid? UrlReferrerCodeId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(20, ErrorMessage = StrText.OverLength)]
        [Display(Name = "代碼")]
        public string Code { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "敘述")]
        public string Description { get; set; }

        public UrlReferrerCodeCreateOrEdit()
        {
        }
    }
    
}
