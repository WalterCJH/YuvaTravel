using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Dashbroad
{
    public class LoginChangePassword : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != PasswordConfirm)
            {
                yield return new ValidationResult("密碼輸入不一致", new string[] { nameof(Password), nameof(PasswordConfirm) });
            }
        }

        public string UserName { get; set; }

        [MaxLength(128)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [MaxLength(128)]
        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        public string PasswordConfirm { get; set; }
    }
}
