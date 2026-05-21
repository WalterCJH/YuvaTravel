using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.FuncPrograms;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IFuncProgramRepository : IBaseRepository<FuncProgram>
    {
        IQueryable<FuncProgram> All(bool isANT = true);
        IQueryable<FuncProgram> IncludeAll(bool isANT = true);
        Task<FuncProgram> FindAsync(string id, bool isANT = true);
        IQueryable<FuncProgram> Search(FuncProgramFilter filter);
        int GetMaxDisplaySeq();
        int GetHundredMaxDisplaySeq();
        bool IsFuncProgramIdRepeat(string userId);
    }
}
