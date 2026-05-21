using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.FuncGroups;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class FuncGroupRepository : BaseRepository<FuncGroup>, IFuncGroupRepository
    {
        public FuncGroupRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<FuncGroup> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<FuncGroup> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.FuncPrograms).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<FuncGroup> FindAsync(string id, bool isANT = true)
        {
            return await IncludeAll(isANT).FirstOrDefaultAsync(p => p.FuncGroupId == id);
        }

        public IQueryable<FuncGroup> Search(FuncGroupFilter filter)
        {
            var data = IncludeAll();

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Name.Contains(filter.Keyword));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }
    }
}
