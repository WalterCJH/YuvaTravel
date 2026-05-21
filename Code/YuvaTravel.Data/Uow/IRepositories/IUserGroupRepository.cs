using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.UserGroups;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IUserGroupRepository : IBaseRepository<UserGroup>
    {
        IQueryable<UserGroup> All(bool isANT = true);
        IQueryable<UserGroup> IncludeAll(bool isANT = true);
        UserGroup Find(string id);
        Task<UserGroup> FindAsync(string id, bool isANT = true);
        IQueryable<UserGroup> Search(UserGroupFilter filter, AuthorizeLevel userAuthorizeLevel);
    }
}
