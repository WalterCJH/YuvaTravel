using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Guides;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IGuideRepository : IBaseRepository<Guide>
    {
        IQueryable<Guide> All(bool isANT = true);
        IQueryable<Guide> IncludeAll(bool isANT = true);
        IQueryable<Guide> QueryActive(bool isANT = true);
        Task<Guide> FindAsync(Guid? id, bool isANT = true);
        Task<Guide> FindCodeAsync(string code, bool isANT = true);
        IQueryable<Guide> Search(GuideFilter filter);
        int GetMaxDisplaySeq();
        bool IsCodeRepeat(string code, Guid? id = null);
        Task ClearOtherFeatured(Guid currentGuideId, string userId);
    }
}
