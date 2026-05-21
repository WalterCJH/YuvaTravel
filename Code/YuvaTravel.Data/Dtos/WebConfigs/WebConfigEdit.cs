using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.WebConfigs
{
    public class WebConfigEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(GoRegister))
            {
                if (GoRegister.Contains("#"))
                {
                    yield return new ValidationResult("不能輸入#", new string[] { nameof(GoRegister) });
                }
                else if (!GoRegister.EndsWith("/"))
                {
                    yield return new ValidationResult("最後必須輸入/", new string[] { nameof(GoRegister) });
                }
            }
        }

        public Guid WebConfigId { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "前往投注")]
        public string GoRegister { get; set; }
    }
}
