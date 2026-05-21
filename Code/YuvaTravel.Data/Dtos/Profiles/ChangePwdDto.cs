using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Uow;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace YuvaTravel.Data.Dtos.Profiles
{

    public class ChangePwdDto : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork))!;
            if (!IsNewUser && !uow.UserRepo.PasswordCheck(UserGuid, OldPassword))
            {
                yield return new ValidationResult("舊密碼輸入錯誤", new string[] { nameof(OldPassword) });
            }

            if (NewPassword != ConfirmPassword)
            {
                yield return new ValidationResult("新密碼與確認密碼不一致", new string[] { nameof(NewPassword), nameof(ConfirmPassword) });
            }
        }

        [Display(Name = "帳號")]
        public Guid UserGuid { get; set; }

        [Display(Name = "新用戶無密碼")]
        public bool IsNewUser { get; set; }

        [Display(Name = "舊密碼")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "新密碼")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "確認密碼")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public ChangePwdDto()
        {
        }
    }
}
