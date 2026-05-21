using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Guides;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IGuideFaqRepository : IBaseRepository<GuideFaq>
    {
        IQueryable<GuideFaq> All(bool isANT = true);
        IQueryable<GuideFaq> IncludeAll(bool isANT = true);
        Task<GuideFaq> FindAsync(Guid? id, bool isANT = true);
        IQueryable<GuideFaq> Search(GuideFaqFilter filter);
        int GetMaxDisplaySeq(Guid? guideId);
    }
}
