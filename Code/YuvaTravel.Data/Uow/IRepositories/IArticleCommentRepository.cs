using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.ArticleComments;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IArticleCommentRepository : IBaseRepository<ArticleComment>
    {
        IQueryable<ArticleComment> All(bool isANT = true);
        IQueryable<ArticleComment> IncludeAll(bool isANT = true);
        Task<ArticleComment> FindAsync(Guid? id, bool isANT = true);
        List<ArticleCommentCreateOrEdit> FindFromArticleId(Guid id, string userId);
    }
}
