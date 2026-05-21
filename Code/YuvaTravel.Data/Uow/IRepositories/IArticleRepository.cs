using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Articles;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IArticleRepository : IBaseRepository<Article>
    {
        IQueryable<Article> All(bool isANT = true);
        IQueryable<Article> IncludeAll(bool isANT = true);

        /// <summary>已通過審核的文章 (所有 ArticleType,含 Include)</summary>
        IQueryable<Article> ActiveAll(bool isANT = true);
        /// <summary>已通過審核 + ArticleType=文章 (ANT)</summary>
        IQueryable<Article> ActiveArticleANT();
        /// <summary>已通過審核 + ArticleType=活動 (ANT)</summary>
        IQueryable<Article> ActiveEventANT();

        Task<Article> FindAsync(Guid? id, bool isANT = true);
        Task<Article> FindCodeAsync(string code, bool isANT = true);

        Task<int> GetMaxSerialNumber();
        int GetMaxDisplaySeq();

        IQueryable<Article> Search(ArticleFilter filter);
        bool IsTitleRepeat(string title, Guid? id);
        bool IsCodeRepeat(string code, Guid? id);
        bool IsAllReply(Guid id);

        /// <summary>排程到期的文章 (用於上線 job)</summary>
        Task<List<Article>> QueryOnScheduleArticleAsync();
    }
}
