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
    public class FuncProgramDetailsOrDelete : FuncProgramDto
    {

        [Display(Name = "代號")]
        public string FuncProgramId { get; set; }

        [Display(Name = "程式群組")]
        public string FuncGroupName { get; set; }

        public FuncProgramDetailsOrDelete()
        {
        }
    }
}
