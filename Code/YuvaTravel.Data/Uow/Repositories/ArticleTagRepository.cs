using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class ArticleTagRepository : BaseRepository<ArticleTag>, IArticleTagRepository
    {
        public ArticleTagRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<ArticleTag> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ArticleTag> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.Article).Include(p => p.Tag).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<ArticleTag> FindAsync(Guid? id, bool isANT = true)
            => await IncludeAll(isANT).FirstOrDefaultAsync(p => p.ArticleTagId == id);

        public void UpdateArticleTagFromArticle(Guid articleId, string[] tagIds)
        {
            // 必須 tracking 才能 Delete,所以走 BaseAll
            var existing = BaseAll().Where(p => p.ArticleId == articleId).ToList();
            foreach (var articleTag in existing)
                Delete(articleTag);

            if (tagIds == null) return;

            int seq = 1;
            foreach (var tagId in tagIds)
            {
                var articleTag = new ArticleTag
                {
                    ArticleTagId = Guid.NewGuid(),
                    ArticleId = articleId,
                    TagId = Guid.Parse(tagId),
                    DisplaySeq = seq++
                };
                Add(articleTag);
            }
        }

        public string[] QueryTagFromArticle(Guid articleId)
        {
            return All()
                .Where(p => p.ArticleId == articleId)
                .OrderBy(p => p.DisplaySeq)
                .Select(p => p.TagId.ToString())
                .ToArray();
        }

        public List<TagDto> GetArticleTags(IEnumerable<ArticleTag> articleTags)
        {
            if (articleTags == null)
                return new List<TagDto>();

            return articleTags
                .Where(p => p.Tag != null)
                .OrderBy(p => p.DisplaySeq)
                .Select(p => new TagDto { Id = p.Tag.TagId, Name = p.Tag.Name, Url = StrPath.Tag(p.Tag.Name) })
                .ToList();
        }
    }
}
