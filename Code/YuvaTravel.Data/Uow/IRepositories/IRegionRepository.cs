using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Regions;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IRegionRepository : IBaseRepository<Region>
    {
        IQueryable<Region> All(bool isANT = true);
        IQueryable<Region> QueryActive(bool isANT = true);
        Task<Region> FindAsync(Guid? id, bool isANT = true);
        Task<Region> FindCodeAsync(string code, bool isANT = true);
        IQueryable<Region> Search(RegionFilter filter);
        int GetMaxDisplaySeq();
        bool IsCodeRepeat(string code, Guid? id = null);
    }
}
