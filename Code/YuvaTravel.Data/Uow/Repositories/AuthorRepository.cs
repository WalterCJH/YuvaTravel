using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Authors;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<Author> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();

            if (isANT) query = query.AsNoTracking();

            return query;
        }

        public IQueryable<Author> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.Guide).AsQueryable();

            if (isANT) query = query.AsNoTracking();

            return query;
        }

        public IQueryable<Author> QueryActive(bool isANT = true) => All(isANT).Where(p => p.IsActive);

        public async Task<Author> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.AuthorId == id);
        }

        public IQueryable<Author> Search(AuthorFilter filter)
        {
            var data = All();

            if (filter.IsActive == WhetherType.是)
                data = data.Where(p => p.IsActive);
            else if (filter.IsActive == WhetherType.否)
                data = data.Where(p => !p.IsActive);

            if (filter.GuideId.HasValue)
                data = data.Where(p => p.GuideId == filter.GuideId.Value);

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Name.Contains(filter.Keyword) || p.NameEn.Contains(filter.Keyword));

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
