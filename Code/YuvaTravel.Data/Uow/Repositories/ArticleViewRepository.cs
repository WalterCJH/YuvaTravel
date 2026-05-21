using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class ArticleViewRepository : BaseRepository<ArticleView>, IArticleViewRepository
    {
        public ArticleViewRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<ArticleView> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ArticleView> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.Article).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<ArticleView> FindAsync(Guid? id, bool isANT = true)
            => await IncludeAll(isANT).FirstOrDefaultAsync(p => p.ArticleViewId == id);
    }
}
