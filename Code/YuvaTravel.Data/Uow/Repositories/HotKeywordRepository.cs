using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.HotKeywords;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class HotKeywordRepository : BaseRepository<HotKeyword>, IHotKeywordRepository
    {
        public HotKeywordRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<HotKeyword> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<HotKeyword> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.HotKeywordId == id);
        }

        public IQueryable<HotKeyword> Search(HotKeywordFilter filter)
        {
            var data = All();

            if (filter.IsActive == WhetherType.否)
                data = data.Where(p => p.IsActive == false);
            else if (filter.IsActive == WhetherType.是)
                data = data.Where(p => p.IsActive == true);

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

        public bool IsNameRepeat(string name, Guid? id = null)
        {
            var data = All().AsQueryable();
            if (!string.IsNullOrEmpty(name))
                data = data.Where(p => p.Name == name);
            if (id != null)
                data = data.Where(p => p.HotKeywordId != id);
            return data.Any();
        }
    }
}
