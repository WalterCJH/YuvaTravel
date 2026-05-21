using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.FuncPrograms;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class FuncProgramRepository : BaseRepository<FuncProgram>, IFuncProgramRepository
    {
        public FuncProgramRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<FuncProgram> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<FuncProgram> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.FuncGroup).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<FuncProgram> FindAsync(string id, bool isANT = true)
        {
            return await IncludeAll(isANT).FirstOrDefaultAsync(p => p.FuncProgramId == id);
        }

        public IQueryable<FuncProgram> Search(FuncProgramFilter filter)
        {
            var data = IncludeAll();

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.FuncProgramId.Contains(filter.Keyword) || p.Name.Contains(filter.Keyword));

            if (filter.AuthorizeLevel != null)
                data = data.Where(p => p.AuthorizeLevel == filter.AuthorizeLevel);

            if (filter.FuncGroupId != null)
                data = data.Where(p => p.FuncGroupId == filter.FuncGroupId);

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }

        public int GetHundredMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + 100 : 10;
        }

        public bool IsFuncProgramIdRepeat(string userId)
        {
            var data = All().AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                data = data.Where(p => p.FuncProgramId == userId);
            return data.Any();
        }
    }
}
