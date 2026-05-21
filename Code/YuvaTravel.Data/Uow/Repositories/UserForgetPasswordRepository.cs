using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class UserForgetPasswordRepository : BaseRepository<UserForgetPassword>, IUserForgetPasswordRepository
    {
        public UserForgetPasswordRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<UserForgetPassword> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public UserForgetPassword Find(Guid id) => BaseAll().FirstOrDefault(p => p.ForgetId == id);

        public async Task<UserForgetPassword> FindAsync(Guid id, bool isANT = true)
            => await All(isANT).FirstOrDefaultAsync(p => p.ForgetId == id);
    }
}
