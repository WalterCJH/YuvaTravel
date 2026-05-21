using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class GuideTagRepository : BaseRepository<GuideTag>, IGuideTagRepository
    {
        public GuideTagRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<GuideTag> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<GuideTag> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.Tag).Include(p => p.Guide).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<GuideTag> FindAsync(Guid? id, bool isANT = true)
            => await IncludeAll(isANT).FirstOrDefaultAsync(p => p.GuideTagId == id);

        public void UpdateGuideTagFromGuide(Guid guideId, string[] tagIds)
        {
            // 需 tracking 才能 Delete
            var existing = BaseAll().Where(p => p.GuideId == guideId).ToList();
            foreach (var item in existing)
                Delete(item);

            if (tagIds == null) return;

            int seq = 1;
            foreach (var tagId in tagIds)
            {
                if (!Guid.TryParse(tagId, out Guid parsed)) continue;
                var gt = new GuideTag
                {
                    GuideTagId = Guid.NewGuid(),
                    GuideId = guideId,
                    TagId = parsed,
                    DisplaySeq = seq++
                };
                Add(gt);
            }
        }

        public string[] QueryTagFromGuide(Guid guideId)
        {
            return All().Where(p => p.GuideId == guideId)
                .OrderBy(p => p.DisplaySeq)
                .Select(p => p.TagId.ToString())
                .ToArray();
        }
    }
}
