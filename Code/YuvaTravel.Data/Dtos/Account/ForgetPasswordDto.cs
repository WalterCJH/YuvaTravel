using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Data.Dtos.Account
{
    public class ForgetPasswordDto : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow == null) yield break;

            var user = uow.UserRepo.FindEmail(Email);
            if (user == null)
            {
                yield return new ValidationResult("Email錯誤", new[] { nameof(Email) });
            }

            if (!uow.UserRepo.IsActive(Email))
            {
                yield return new ValidationResult("帳號未啟用", new[] { nameof(Email) });
            }
        }

        [Required(ErrorMessage = StrText.Required)]
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}
