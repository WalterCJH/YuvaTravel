using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.ArticleCategories;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IArticleCategoryRepository : IBaseRepository<ArticleCategory>
    {
        IQueryable<ArticleCategory> All(bool isANT = true);
        Task<ArticleCategory> FindAsync(Guid? id, bool isANT = true);
        IQueryable<ArticleCategory> Search(ArticleCategoryFilter filter);
        int GetMaxDisplaySeq();
        bool IsCodeRepeat(string code, Guid? id = null);
    }
}
