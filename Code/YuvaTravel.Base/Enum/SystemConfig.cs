using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Base.Enum
{
    public enum SystemConfig : int
    {
        Develop = 0,
        Integration = 1,
        Testing = 2,
        Quality = 3,
        Staging = 4,
        Production = 5
    }
}
