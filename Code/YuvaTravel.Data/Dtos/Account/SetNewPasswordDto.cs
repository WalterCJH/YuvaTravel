using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.Account
{
    public class SetNewPasswordDto : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NewPassword != ConfirmPassword)
            {
                yield return new ValidationResult("新密碼與確認密碼不一致", new[] { nameof(NewPassword), nameof(ConfirmPassword) });
            }
        }

        [Required(ErrorMessage = StrText.Required)]
        [DisplayName("ID")]
        public Guid ForgetId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [DisplayName("新密碼")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [DisplayName("確認密碼")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
