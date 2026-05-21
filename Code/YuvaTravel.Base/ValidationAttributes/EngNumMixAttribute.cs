using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YuvaTravel.Base.ValidationAttributes
{
    //public class ModelClientValidationEngNumMixRule : ModelClientValidationRule
    //{
    //    public ModelClientValidationEngNumMixRule(string errorMessage, object pattern)
    //    {
    //        ErrorMessage = errorMessage;
    //        //ValidationType = "range"; // 這是 jQuery Unobtrusive validation 內建支援的類型
    //        ValidationParameters["pattern"] = pattern;
    //    }
    //}

    //public class EngNumMixAttributeAdapter : DataAnnotationsModelValidator<RegularExpressionAttribute>
    //{
    //    public EngNumMixAttributeAdapter(ModelMetadata metadata, ControllerContext context, EngNumMixAttribute attribute)
    //        : base(metadata, context, attribute)
    //    {
    //    }

    //    public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
    //    {
    //        string errorMessage = ErrorMessage;
    //        return new[] { new ModelClientValidationEngNumMixRule(errorMessage, Attribute.Pattern) };
    //    }
    //}

    public class EngNumMixAttribute : RegularExpressionAttribute
    {
        //static EngNumMixAttribute()
        //{
        //    DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EngNumMixAttribute), typeof(RegularExpressionAttributeAdapter));
        //}
        public EngNumMixAttribute() : base(@"[A-Za-z0-9]+")
        {
            base.ErrorMessage = "請輸入英數字";
        }
    }
}
