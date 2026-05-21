using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Dtos.Articles;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class ArticleSDCommonQuestionRepository : BaseRepository<ArticleSDCommonQuestion>, IArticleSDCommonQuestionRepository
    {
        public ArticleSDCommonQuestionRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<ArticleSDCommonQuestion> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ArticleSDCommonQuestion> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.Article).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<ArticleSDCommonQuestion> FindAsync(Guid? id, bool isANT = true)
            => await IncludeAll(isANT).FirstOrDefaultAsync(p => p.ArticleSDCommonQuestionId == id);

        /// <summary>取得整篇文章的 SD entity 集合。預設 tracking (因為呼叫端常用於 Delete + 重建)。</summary>
        public async Task<List<ArticleSDCommonQuestion>> QueryEntityFromArticleId(Guid id, bool isANT = false)
            => await All(isANT).Where(p => p.ArticleId == id).ToListAsync();

        public List<ArticleSDCommonQuestionCreateOrEdit> QueryFromArticleId(Guid id)
        {
            return All().Where(p => p.ArticleId == id)
                .OrderBy(p => p.DisplaySequence)
                .Select(p => new ArticleSDCommonQuestionCreateOrEdit
                {
                    ArticleSDQuestionId = p.ArticleSDCommonQuestionId,
                    Name = p.Name,
                    Text = p.Text
                }).ToList();
        }

        public async Task<List<ArticleSDCSDto>> QueryShowFromArticleId(Guid id)
        {
            return await All().Where(p => p.ArticleId == id)
                .OrderBy(p => p.DisplaySequence)
                .Select(p => new ArticleSDCSDto
                {
                    Name = p.Name,
                    Text = p.Text
                }).ToListAsync();
        }
    }
}
