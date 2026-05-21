using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IArticleViewRepository : IBaseRepository<ArticleView>
    {
        IQueryable<ArticleView> All(bool isANT = true);
        IQueryable<ArticleView> IncludeAll(bool isANT = true);
        Task<ArticleView> FindAsync(Guid? id, bool isANT = true);
    }
}
