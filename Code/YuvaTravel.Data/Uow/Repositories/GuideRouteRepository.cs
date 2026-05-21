using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.Guides;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class GuideRouteRepository : BaseRepository<GuideRoute>, IGuideRouteRepository
    {
        public GuideRouteRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<GuideRoute> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<GuideRoute> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.Guide).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<GuideRoute> FindAsync(Guid? id, bool isANT = true)
        {
            return await IncludeAll(isANT).FirstOrDefaultAsync(p => p.GuideRouteId == id);
        }

        public IQueryable<GuideRoute> Search(GuideRouteFilter filter)
        {
            var data = IncludeAll();
            if (filter.GuideId.HasValue)
                data = data.Where(p => p.GuideId == filter.GuideId.Value);
            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Title.Contains(filter.Keyword));
            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq(Guid? guideId)
        {
            var query = All();
            if (guideId.HasValue) query = query.Where(p => p.GuideId == guideId.Value);
            var data = query.OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }
    }
}
