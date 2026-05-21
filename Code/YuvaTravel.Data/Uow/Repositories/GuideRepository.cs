using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Guides;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class GuideRepository : BaseRepository<Guide>, IGuideRepository
    {
        public GuideRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<Guide> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<Guide> IncludeAll(bool isANT = true)
        {
            var query = BaseAll()
                .Include(p => p.GuideTags).ThenInclude(p => p.Tag)
                .Include(p => p.GuideRoutes)
                .Include(p => p.GuideReviews)
                .Include(p => p.GuideFaqs)
                .Include(p => p.Authors).ThenInclude(a => a.Articles)
                .Include(p => p.GuideRegions).ThenInclude(p => p.Region)
                .AsQueryable();

            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<Guide> QueryActive(bool isANT = true) => IncludeAll(isANT).Where(p => p.IsActive);

        public async Task<Guide> FindAsync(Guid? id, bool isANT = true)
        {
            return await IncludeAll(isANT).FirstOrDefaultAsync(p => p.GuideId == id);
        }

        public async Task<Guide> FindCodeAsync(string code, bool isANT = true)
        {
            return await IncludeAll(isANT).FirstOrDefaultAsync(p => p.Code == code);
        }

        public IQueryable<Guide> Search(GuideFilter filter)
        {
            var data = IncludeAll();

            if (filter.IsActive == WhetherType.是)
                data = data.Where(p => p.IsActive);
            else if (filter.IsActive == WhetherType.否)
                data = data.Where(p => !p.IsActive);

            if (filter.IsFeatured == WhetherType.是)
                data = data.Where(p => p.IsFeatured);
            else if (filter.IsFeatured == WhetherType.否)
                data = data.Where(p => !p.IsFeatured);

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Name.Contains(filter.Keyword) || p.NameEn.Contains(filter.Keyword) || p.Code.Contains(filter.Keyword));

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
                data = data.Where(p => p.GuideId != id);
            return data.Any();
        }

        /// <summary>把目前以外的其他嚮導 IsFeatured 設為 false (本月精選只能一位)。需 tracking 才能寫回。</summary>
        public async Task ClearOtherFeatured(Guid currentGuideId, string userId)
        {
            var others = await All(isANT: false)
                .Where(p => p.GuideId != currentGuideId && p.IsFeatured)
                .ToListAsync();
            foreach (var g in others)
            {
                g.IsFeatured = false;
                g.UserLog.UpdateTime = DateTime.Now;
                g.UserLog.UpdateUserId = userId;
            }
        }
    }
}
