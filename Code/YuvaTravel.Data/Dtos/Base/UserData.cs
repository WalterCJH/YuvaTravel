using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Base
{
    public class UserData
    {
        public Guid UserGuid { get; set; }
        public string UserId { get; set; }
        public string UserGroupId { get; set; }
        public string UserName { get; set; }
        public AuthorizeLevel AuthorizeLevel { get; set; } = AuthorizeLevel.一般管理員;
        public bool IsSystemLevel
        {
            get
            {
                if (AuthorizeLevel == AuthorizeLevel.系統管理員)
                {
                    return true;
                }
                return false;
            }
        }
        public bool IsAdminLevel
        {
            get
            {
                if (AuthorizeLevel == AuthorizeLevel.後台管理員)
                {
                    return true;
                }
                return false;
            }
        }
        public string Role { get; set; }
        //public List<FuncGroupDto> FuncGroups { get; set; }
    }
}
