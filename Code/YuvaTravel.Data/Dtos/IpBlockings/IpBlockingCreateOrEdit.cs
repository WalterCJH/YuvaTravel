using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Base.ValidationAttributes;
using YuvaTravel.Data.Uow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.IpBlockings
{
    public class IpBlockingCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow == null) yield break;

            IpAddress = IpAddress?.Trim();
            if (uow.IpBlockingRepo.IsIpRepeat(IpAddress, IpBlockingId))
            {
                yield return new ValidationResult(StrText.IpNotRepeat, new[] { nameof(IpAddress) });
            }

            if (uow.IpBlockingRepo.IsIncompleteIP(IpAddress))
            {
                IsIncompleteIP = true;
                IpAddress = uow.IpBlockingRepo.IncompleteIPAddDot(IpAddress);
            }
        }

        [Display(Name = "ID")]
        public Guid? IpBlockingId { get; set; }

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
