using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Guides;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IGuideRouteRepository : IBaseRepository<GuideRoute>
    {
        IQueryable<GuideRoute> All(bool isANT = true);
        IQueryable<GuideRoute> IncludeAll(bool isANT = true);
        Task<GuideRoute> FindAsync(Guid? id, bool isANT = true);
        IQueryable<GuideRoute> Search(GuideRouteFilter filter);
        int GetMaxDisplaySeq(Guid? guideId);
    }
}
