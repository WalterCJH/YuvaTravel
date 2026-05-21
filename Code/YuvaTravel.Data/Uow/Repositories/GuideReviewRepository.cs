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
    public class GuideReviewRepository : BaseRepository<GuideReview>, IGuideReviewRepository
    {
        public GuideReviewRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<GuideReview> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<GuideReview> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.Guide).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<GuideReview> FindAsync(Guid? id, bool isANT = true)
        {
            return await IncludeAll(isANT).FirstOrDefaultAsync(p => p.GuideReviewId == id);
        }

        public IQueryable<GuideReview> Search(GuideReviewFilter filter)
        {
            var data = IncludeAll();
            if (filter.GuideId.HasValue)
                data = data.Where(p => p.GuideId == filter.GuideId.Value);
            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.ReviewerName.Contains(filter.Keyword) || p.Content.Contains(filter.Keyword));
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
