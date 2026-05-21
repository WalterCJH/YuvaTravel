using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Articles;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class ArticleRepository : BaseRepository<Article>, IArticleRepository
    {
        public ArticleRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<Article> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<Article> IncludeAll(bool isANT = true)
        {
            var query = BaseAll()
                .Include(p => p.ArticleCategoryArticles).ThenInclude(p => p.ArticleCategory)
                .Include(p => p.ArticleTags).ThenInclude(p => p.Tag)
                .Include(p => p.Author).ThenInclude(p => p.Guide).ThenInclude(p => p.GuideRegions).ThenInclude(p => p.Region)
                .Include(p => p.ArticleRegions).ThenInclude(p => p.Region)
                .AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<Article> ActiveAll(bool isANT = true)
            => IncludeAll(isANT).Where(p => p.ArticleReviewType == ArticleReviewType.已通過);

        public IQueryable<Article> ActiveArticleANT()
            => IncludeAll(true).Where(p => p.ArticleReviewType == ArticleReviewType.已通過 && p.ArticleType == ArticleType.文章);

        public IQueryable<Article> ActiveEventANT()
            => IncludeAll(true).Where(p => p.ArticleReviewType == ArticleReviewType.已通過 && p.ArticleType == ArticleType.活動);

        public async Task<Article> FindAsync(Guid? id, bool isANT = true)
            => await IncludeAll(isANT).FirstOrDefaultAsync(p => p.ArticleId == id);

        public async Task<Article> FindCodeAsync(string code, bool isANT = true)
            => await IncludeAll(isANT).FirstOrDefaultAsync(p => p.Code == code);

        public async Task<int> GetMaxSerialNumber()
        {
            var article = await BaseAll().AsNoTracking().OrderByDescending(o => o.SerialNumber).FirstOrDefaultAsync();
            return article != null ? article.SerialNumber + 1 : 1;
        }

        public int GetMaxDisplaySeq()
        {
            var data = BaseAll().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }

        public IQueryable<Article> Search(ArticleFilter filter)
        {
            var data = IncludeAll();

            if (filter.IsTop == WhetherType.否)
                data = data.Where(p => p.IsTop == false);
            else if (filter.IsTop == WhetherType.是)
                data = data.Where(p => p.IsTop == true);

            if (filter.IsFeaturedArticle == WhetherType.否)
                data = data.Where(p => p.IsFeaturedArticle == false);
            else if (filter.IsFeaturedArticle == WhetherType.是)
                data = data.Where(p => p.IsFeaturedArticle == true);

            if (filter.ArticleReviewType != null)
                data = data.Where(p => p.ArticleReviewType == filter.ArticleReviewType);

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Title.Contains(filter.Keyword) || p.Code.Contains(filter.Keyword));

            if (!string.IsNullOrEmpty(filter.Author))
                data = data.Where(p => p.UserLog.CreateUserId.Contains(filter.Author));

            if (filter.ArticleCategoryId != null)
                data = data.Where(p => p.ArticleCategoryArticles.Any(a => a.ArticleCategoryId == filter.ArticleCategoryId));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public bool IsTitleRepeat(string title, Guid? id)
        {
            var data = BaseAll().AsQueryable();
            if (!string.IsNullOrEmpty(title))
                data = data.Where(p => p.Title == title);
            if (id != null)
                data = data.Where(p => p.ArticleId != id);
            return data.Any();
        }

        public bool IsCodeRepeat(string code, Guid? id)
        {
            var data = BaseAll().AsQueryable();
            if (!string.IsNullOrEmpty(code))
                data = data.Where(p => p.Code == code);
            if (id != null)
                data = data.Where(p => p.ArticleId != id);
            return data.Any();
        }

        public bool IsAllReply(Guid id)
        {
            var notReplyCount = All().Where(p => p.ArticleId == id)
                .Count(p => p.ArticleComments.Any(a => a.ReplyTime == null));
            return notReplyCount == 0;
        }

        public async Task<List<Article>> QueryOnScheduleArticleAsync()
        {
            return await IncludeAll(false)   // 上線排程要 tracking 才能改 OnlineTime
                .Where(p => p.ArticleReviewType == ArticleReviewType.排程中 && p.OnScheduleTime < DateTime.Now)
                .ToListAsync();
        }
    }
}
