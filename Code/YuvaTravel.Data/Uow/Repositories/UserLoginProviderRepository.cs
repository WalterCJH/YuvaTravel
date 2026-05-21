using System.Linq;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class UserLoginProviderRepository : BaseRepository<UserLoginProvider>, IUserLoginProviderRepository
    {
        public UserLoginProviderRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<UserLoginProvider> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<UserLoginProvider> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.User).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }
    }
}
