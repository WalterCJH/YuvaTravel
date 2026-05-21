using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Articles;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IArticleSDCommonQuestionRepository : IBaseRepository<ArticleSDCommonQuestion>
    {
        IQueryable<ArticleSDCommonQuestion> All(bool isANT = true);
        IQueryable<ArticleSDCommonQuestion> IncludeAll(bool isANT = true);
        Task<ArticleSDCommonQuestion> FindAsync(Guid? id, bool isANT = true);
        Task<List<ArticleSDCommonQuestion>> QueryEntityFromArticleId(Guid id, bool isANT = false);
        List<ArticleSDCommonQuestionCreateOrEdit> QueryFromArticleId(Guid id);
        Task<List<ArticleSDCSDto>> QueryShowFromArticleId(Guid id);
    }
}
