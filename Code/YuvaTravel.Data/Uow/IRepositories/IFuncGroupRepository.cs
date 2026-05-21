using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.FuncGroups;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IFuncGroupRepository : IBaseRepository<FuncGroup>
    {
        IQueryable<FuncGroup> All(bool isANT = true);
        IQueryable<FuncGroup> IncludeAll(bool isANT = true);
        Task<FuncGroup> FindAsync(string id, bool isANT = true);
        IQueryable<FuncGroup> Search(FuncGroupFilter filter);
        int GetMaxDisplaySeq();
    }
}
