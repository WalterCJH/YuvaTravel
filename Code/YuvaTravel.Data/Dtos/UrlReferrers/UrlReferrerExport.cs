using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.UrlReferrers
{
    public class UrlReferrerExport : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //DateTime dt = new DateTime(2021, 1, 1);
            //if (StartTime < dt)
            //{
            //    yield return new ValidationResult("開始時間必須晚於2021年", new string[] { nameof(StartTime) });
            //}
            //if (EndTime < dt)
            //{
            //    yield return new ValidationResult("結束時間必須晚於2021年", new string[] { nameof(EndTime) });
            //}
            //if (StartTime >= EndTime)
            //{
            //    yield return new ValidationResult("開始時間必須早於結束時間", new string[] { nameof(StartTime) });
            //}
            if (!string.IsNullOrWhiteSpace(RangeTime))
            {
                var strYear = RangeTime.Substring(0, 4);
                if (int.TryParse(strYear, out int year))
                {
                    if (year < 2021)
                    {
                        yield return new ValidationResult("開始時間必須晚於2021年", new string[] { nameof(RangeTime) });
                    }
                }
            }
        }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "來源時間")]
        public string RangeTime { get; set; }
    }
}
