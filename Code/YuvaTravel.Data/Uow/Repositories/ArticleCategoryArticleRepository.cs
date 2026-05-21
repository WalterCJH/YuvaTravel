using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Dtos.Articles;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class ArticleCategoryArticleRepository : BaseRepository<ArticleCategoryArticle>, IArticleCategoryArticleRepository
    {
        public ArticleCategoryArticleRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<ArticleCategoryArticle> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public ArticleCategoryArticle Find(Guid id) => BaseAll().FirstOrDefault(p => p.Id == id);

        public async Task<ArticleCategoryArticle> FindAsync(Guid id, bool isANT = true)
            => await All(isANT).FirstOrDefaultAsync(p => p.Id == id);

        public async Task UpdateArticleCategoryArticle(List<ArticleCategoryDto> articleCategories, Guid articleId, string userId)
        {
            // 需 tracking 才能 Delete,所以走 BaseAll
            var existing = await BaseAll().Where(p => p.ArticleId == articleId).ToListAsync();

            foreach (var category in articleCategories)
            {
                if (category.IsActive)
                {
                    if (!existing.Any(c => c.ArticleCategoryId == category.ArticleCategoryId))
                    {
                        var entity = new ArticleCategoryArticle
                        {
                            Id = Guid.NewGuid(),
                            ArticleCategoryId = category.ArticleCategoryId,
                            ArticleId = articleId
                        };
                        entity.UserLog.CreateTime = DateTime.Now;
                        entity.UserLog.CreateUserId = userId;
                        Add(entity);
                    }
                }
                else
                {
                    var entity = existing.FirstOrDefault(c => c.ArticleCategoryId == category.ArticleCategoryId);
                    if (entity != null)
                        Delete(entity);
                }
            }
        }
    }
}
