using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Base.ValidationAttributes;
using YuvaTravel.Data.Uow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.IpOpenings
{
    public class IpOpeningCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow == null) yield break;

            IpAddress = IpAddress?.Trim();
            if (uow.IpOpeningRepo.IsIpRepeat(IpAddress, IpOpeningId))
            {
                yield return new ValidationResult(StrText.IpNotRepeat, new[] { nameof(IpAddress) });
            }

            if (uow.IpOpeningRepo.IsIncompleteIP(IpAddress))
            {
                IsIncompleteIP = true;
                IpAddress = uow.IpOpeningRepo.IncompleteIPAddDot(IpAddress);
            }
        }

        [Display(Name = "ID")]
        public Guid? IpOpeningId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "IP位置")]
        public string IpAddress { get; set; }

        [Display(Name = "是否為不完整IP")]
        public bool IsIncompleteIP { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "備註")]
        public string Memo { get; set; }

    }
}
