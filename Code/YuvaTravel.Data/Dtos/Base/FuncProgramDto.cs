using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Base
{
    public class FuncProgramDto
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool OnlyAdmin { get; set; }
        public int DisplaySeq { get; set; }
    }
}
