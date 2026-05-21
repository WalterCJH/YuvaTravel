using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Base
{
    public class FuncGroupDto
    {
        public string Name { get; set; }
        public string IconClass { get; set; }
        public int DisplaySeq { get; set; }
        public List<FuncProgramDto> FuncPrograms { get; set; }

        public FuncGroupDto()
        {
            FuncPrograms = new List<FuncProgramDto>();
        }
    }
}
