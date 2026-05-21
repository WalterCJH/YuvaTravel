using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Dtos.UrlReferrerCodes;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class UrlReferrerCodeRepository : BaseRepository<UrlReferrerCode>, IUrlReferrerCodeRepository
    {
        public UrlReferrerCodeRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<UrlReferrerCode> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public UrlReferrerCode Find(Guid? id, bool isANT = true)
        {
            return All().FirstOrDefault(p => p.UrlReferrerCodeId == id);
        }

        public async Task<UrlReferrerCode> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.UrlReferrerCodeId == id);
        }

        public IQueryable<UrlReferrerCode> Search(UrlReferrerCodeFilter filter)
        {
            var data = All();
            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public bool IsCodeRepeat(string code, Guid? id)
        {
            var data = All();
            if (!string.IsNullOrEmpty(code))
                data = data.Where(p => p.Code == code);
            if (id != null)
                data = data.Where(p => p.UrlReferrerCodeId != id);
            return data.Any();
        }
    }
}
