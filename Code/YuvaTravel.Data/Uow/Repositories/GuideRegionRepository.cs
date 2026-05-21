using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Dtos.Regions;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class GuideRegionRepository : BaseRepository<GuideRegion>, IGuideRegionRepository
    {
        public GuideRegionRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<GuideRegion> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task UpdateGuideRegions(List<RegionPickDto> regions, Guid guideId, string userId)
        {
            var existing = await BaseAll().Where(p => p.GuideId == guideId).ToListAsync();

            foreach (var region in regions)
            {
                if (region.IsActive)
                {
                    if (!existing.Any(c => c.RegionId == region.RegionId))
                    {
                        var entity = new GuideRegion
                        {
                            Id = Guid.NewGuid(),
                            GuideId = guideId,
                            RegionId = region.RegionId
                        };
                        entity.UserLog.CreateTime = DateTime.Now;
                        entity.UserLog.CreateUserId = userId;
                        Add(entity);
                    }
                }
                else
                {
                    var entity = existing.FirstOrDefault(c => c.RegionId == region.RegionId);
                    if (entity != null)
                        Delete(entity);
                }
            }
        }
    }
}
