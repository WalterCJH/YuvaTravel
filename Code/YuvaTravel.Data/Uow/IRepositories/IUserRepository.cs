using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Base;
using YuvaTravel.Data.Dtos.Users;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        IQueryable<User> All(bool isANT = true);
        IQueryable<User> IncludeAll(bool isANT = true);

        User FindGuid(Guid id);
        Task<User> FindGuidAsync(Guid id, bool isANT = true);
        User Find(string id);
        Task<User> FindAsync(string id, bool isANT = true);
        User FindEmail(string email);

        IQueryable<User> Search(UserFilter filter, string userGroupId);

        bool IsUserIdRepeat(string userId);
        bool IsUserEmailRepeat(string email, Guid? userGuid = null);

        bool PasswordCheck(string email, string password);
        bool PasswordCheck(Guid userGuid, string password);
        bool IsNoPassword(Guid userGuid);
        bool IsActive(string email);

        List<FuncGroupDto> QueryFuncGroupForAuthorize(Guid userGuid);
        string GetNotRepeatUserId(string userId);
        string HashPassword(string password);
    }
}
