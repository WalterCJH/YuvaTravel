using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Regions;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class RegionRepository : BaseRepository<Region>, IRegionRepository
    {
        public RegionRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<Region> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<Region> QueryActive(bool isANT = true) => All(isANT).Where(p => p.IsActive);

        public async Task<Region> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.RegionId == id);
        }

        public async Task<Region> FindCodeAsync(string code, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.Code == code);
        }

        public IQueryable<Region> Search(RegionFilter filter)
        {
            var data = All();

            if (filter.IsActive == WhetherType.是)
                data = data.Where(p => p.IsActive);
            else if (filter.IsActive == WhetherType.否)
                data = data.Where(p => !p.IsActive);

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Code.Contains(filter.Keyword) || p.Name.Contains(filter.Keyword) || p.NameEn.Contains(filter.Keyword));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }

        public bool IsCodeRepeat(string code, Guid? id = null)
        {
            var data = All();
            if (!string.IsNullOrEmpty(code))
                data = data.Where(p => p.Code == code);
            if (id != null)
                data = data.Where(p => p.RegionId != id);
            return data.Any();
        }
    }
}
