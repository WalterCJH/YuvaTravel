using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YuvaTravel.Base.ValidationAttributes
{
    public class ColorCodeAttribute : RegularExpressionAttribute
    {
        public ColorCodeAttribute() : base(@"#[A-Za-z0-9]{6}")
        {
            base.ErrorMessage = "請輸入正確的顏色代碼";
        }
    }
}
