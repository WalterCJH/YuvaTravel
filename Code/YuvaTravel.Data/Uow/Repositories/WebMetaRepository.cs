using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.WebMetas;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class WebMetaRepository : BaseRepository<WebMeta>, IWebMetaRepository
    {
        public WebMetaRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<WebMeta> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<WebMeta> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.WebMetaId == id);
        }

        public IQueryable<WebMeta> Search(WebMetaFilter filter)
        {
            var data = All();

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Name.Contains(filter.Keyword) || p.MetaTitle.Contains(filter.Keyword));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }

        public bool IsUrlRepeat(string url, Guid? id)
        {
            var data = All();
            if (!string.IsNullOrEmpty(url))
                data = data.Where(p => p.Url == url);
            if (id != null)
                data = data.Where(p => p.WebMetaId != id);
            return data.Any();
        }
    }
}
