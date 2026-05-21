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
    public class ArticleRegionRepository : BaseRepository<ArticleRegion>, IArticleRegionRepository
    {
        public ArticleRegionRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<ArticleRegion> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task UpdateArticleRegions(List<RegionPickDto> regions, Guid articleId, string userId)
        {
            var existing = await BaseAll().Where(p => p.ArticleId == articleId).ToListAsync();

            foreach (var region in regions)
            {
                if (region.IsActive)
                {
                    if (!existing.Any(c => c.RegionId == region.RegionId))
                    {
                        var entity = new ArticleRegion
                        {
                            Id = Guid.NewGuid(),
                            ArticleId = articleId,
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
