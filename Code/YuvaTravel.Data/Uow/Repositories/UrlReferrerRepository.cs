using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class UrlReferrerRepository : BaseRepository<UrlReferrer>, IUrlReferrerRepository
    {
        public UrlReferrerRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<UrlReferrer> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public UrlReferrer Find(Guid? id, bool isANT = true)
        {
            return All(isANT).FirstOrDefault(p => p.UrlReferrerId == id);
        }

        public async Task<UrlReferrer> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.UrlReferrerId == id);
        }
    }
}
