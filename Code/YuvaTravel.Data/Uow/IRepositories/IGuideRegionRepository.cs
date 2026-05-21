using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Regions;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IGuideRegionRepository : IBaseRepository<GuideRegion>
    {
        IQueryable<GuideRegion> All(bool isANT = true);
        Task UpdateGuideRegions(List<RegionPickDto> regions, Guid guideId, string userId);
    }
}
