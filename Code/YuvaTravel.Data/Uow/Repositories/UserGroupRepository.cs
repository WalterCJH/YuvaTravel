using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.UserGroups;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class UserGroupRepository : BaseRepository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<UserGroup> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<UserGroup> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.UserGroupFuncPrograms).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public UserGroup Find(string id) => IncludeAll(false).FirstOrDefault(p => p.UserGroupId == id);

        public async Task<UserGroup> FindAsync(string id, bool isANT = true)
        {
            return await IncludeAll(isANT).FirstOrDefaultAsync(p => p.UserGroupId == id);
        }

        public IQueryable<UserGroup> Search(UserGroupFilter filter, AuthorizeLevel userAuthorizeLevel)
        {
            var data = IncludeAll();

            data = data.Where(p => p.AuthorizeLevel <= userAuthorizeLevel);

            if (filter.AuthorizeLevel != null)
                data = data.Where(p => p.AuthorizeLevel == filter.AuthorizeLevel);

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.UserGroupId.Contains(filter.Keyword) || p.Name.Contains(filter.Keyword));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }
    }
}
