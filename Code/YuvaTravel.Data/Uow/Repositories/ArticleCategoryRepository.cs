using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.ArticleCategories;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class ArticleCategoryRepository : BaseRepository<ArticleCategory>, IArticleCategoryRepository
    {
        public ArticleCategoryRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<ArticleCategory> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<ArticleCategory> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.ArticleCategoryId == id);
        }

        public IQueryable<ArticleCategory> Search(ArticleCategoryFilter filter)
        {
            var data = All();

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Code.Contains(filter.Keyword) || p.Name.Contains(filter.Keyword));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }

        public bool IsCodeRepeat(string code, Guid? id = null)
        {
            var data = All();
            if (!string.IsNullOrEmpty(code))
                data = data.Where(p => p.Code == code);
            if (id != null)
                data = data.Where(p => p.ArticleCategoryId != id);
            return data.Any();
        }
    }
}
