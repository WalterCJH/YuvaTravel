using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Uow;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace YuvaTravel.Data.Dtos.Users
{
    public class UserDto : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow == null) yield break;

            if (!string.IsNullOrWhiteSpace(Email) && uow.UserRepo.IsUserEmailRepeat(Email, UserGuid))
            {
                yield return new ValidationResult(StrText.EmailNotRepeat, new[] { nameof(Email) });
            }
            if (UserGuid == null && string.IsNullOrWhiteSpace(_Password))
            {
                yield return new ValidationResult("密碼必須輸入", new[] { nameof(_Password) });
            }
        }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [Display(Name = "登入變更密碼")]
        public bool LoginResetPassword { get; set; }

        public Guid? UserGuid { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(30)]
        [Display(Name = "姓")]
        public string LastName { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(30)]
        [Display(Name = "名")]
        public string FirstName { get; set; }

        [Display(Name = "姓名")]
        public string Name
        {
            get { return string.Format("{0}{1}", LastName, FirstName); }
        }

        [MaxLength(10)]
        [Display(Name = "手機")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [EmailAddress]
        [MaxLength(100)]
        [Display(Name = "Email (即為登入帳號)")]
        public string Email { get; set; }

        [MaxLength(128)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string _Password { get; set; }

        public UserDto()
        {
        }
    }
}
