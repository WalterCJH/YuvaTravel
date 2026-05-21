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
using YuvaTravel.Base.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace YuvaTravel.Data.Dtos.FuncPrograms
{
    [MetadataType(typeof(FuncProgramDto))]
    public class FuncProgramCreateOrEdit : FuncProgramDto
    {
        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "代號")]
        public string FuncProgramId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Column(TypeName = "VARCHAR")]
        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "程式群組")]
        public string FuncGroupId { get; set; }

        public FuncProgramCreateOrEdit()
        {
        }
    }
}
