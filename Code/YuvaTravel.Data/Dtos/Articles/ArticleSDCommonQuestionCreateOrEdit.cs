using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YuvaTravel.Data.Dtos.Articles
{

    public class ArticleSDCommonQuestionCreateOrEdit
    {
        [Display(Name = "ID")]
        public Guid? ArticleSDQuestionId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "題目")]
        public string Name { get; set; }

        
        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(4000, ErrorMessage = StrText.OverLength)]
        [Display(Name = "答案")]
        public string Text { get; set; }
    }

}
