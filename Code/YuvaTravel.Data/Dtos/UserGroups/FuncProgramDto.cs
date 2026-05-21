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

namespace YuvaTravel.Data.Dtos.UserGroups
{

    public class FuncProgramDto
    {
        public ProgramId ProgramId { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveCreate { get; set; }
        public bool IsCreate { get; set; }
        public bool IsActiveEdit { get; set; }
        public bool IsEdit { get; set; }
        public bool IsActiveDelete { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActiveDetails { get; set; }
        public bool IsDetails { get; set; }
        public bool IsActiveImport { get; set; }
        public bool IsImport { get; set; }
        public bool IsActiveExport { get; set; }
        public bool IsExport { get; set; }
        public string Name { get; set; }
        public int DisplaySeq { get; set; }
    }
}
