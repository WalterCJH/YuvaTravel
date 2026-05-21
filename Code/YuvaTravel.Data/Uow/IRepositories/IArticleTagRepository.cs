using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IArticleTagRepository : IBaseRepository<ArticleTag>
    {
        IQueryable<ArticleTag> All(bool isANT = true);
        IQueryable<ArticleTag> IncludeAll(bool isANT = true);
        Task<ArticleTag> FindAsync(Guid? id, bool isANT = true);

        /// <summary>清掉此文章現有的 ArticleTag,再依 tagIds 重建。需要呼叫端後續 CommitAsync。</summary>
        void UpdateArticleTagFromArticle(Guid articleId, string[] tagIds);

        string[] QueryTagFromArticle(Guid articleId);
        List<TagDto> GetArticleTags(IEnumerable<ArticleTag> articleTags);
    }
}
