using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Regions;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IArticleRegionRepository : IBaseRepository<ArticleRegion>
    {
        IQueryable<ArticleRegion> All(bool isANT = true);

        /// <summary>依 dto 重建文章與地區的關聯。需要呼叫端後續 CommitAsync。</summary>
        Task UpdateArticleRegions(List<RegionPickDto> regions, Guid articleId, string userId);
    }
}
