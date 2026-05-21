using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.UserGroups;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IUserGroupFuncProgramRepository : IBaseRepository<UserGroupFuncProgram>
    {
        IQueryable<UserGroupFuncProgram> All(bool isANT = true);
        UserGroupFuncProgram Find(Guid id);
        Task<UserGroupFuncProgram> FindAsync(Guid id, bool isANT = true);
        Task UpdateUserGroupFuncProgram(List<FuncGroupDto> funcGroups, UserGroup userGroup, string userId);
    }
}
