using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Data.Dtos.Account
{
    public class LoginDto : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow == null) yield break;

            if (uow.UserRepo.PasswordCheck(Email, Password))
            {
                yield return ValidationResult.Success;
            }
            else
            {
                yield return new ValidationResult("帳號或密碼錯誤", new[] { nameof(Email), nameof(Password) });
            }

            if (!uow.UserRepo.IsActive(Email))
            {
                yield return new ValidationResult("帳號未啟用", new[] { nameof(Email) });
            }
        }

        [Required(ErrorMessage = StrText.Required)]
        [DisplayName("帳號 (請輸入Email)")]
        public string Email { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [DisplayName("密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("記得我")]
        public bool RememberMe { get; set; }

        [DisplayName("導回頁")]
        public string ReturnUrl { get; set; }

        [DisplayName("錯誤訊息")]
        public string ErrorMsg { get; set; }
    }
}
