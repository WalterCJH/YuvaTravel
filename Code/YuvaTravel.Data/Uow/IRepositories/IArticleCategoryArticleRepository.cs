using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Articles;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IArticleCategoryArticleRepository : IBaseRepository<ArticleCategoryArticle>
    {
        IQueryable<ArticleCategoryArticle> All(bool isANT = true);
        ArticleCategoryArticle Find(Guid id);
        Task<ArticleCategoryArticle> FindAsync(Guid id, bool isANT = true);

        /// <summary>依 dto 重建文章與類別的關聯。需要呼叫端後續 CommitAsync。</summary>
        Task UpdateArticleCategoryArticle(List<ArticleCategoryDto> articleCategories, Guid articleId, string userId);
    }
}
