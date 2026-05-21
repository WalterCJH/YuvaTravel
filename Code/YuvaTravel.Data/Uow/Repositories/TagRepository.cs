using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Dtos.Tags;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class TagRepository : BaseRepository<Tag>, ITagRepository
    {
        public TagRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<Tag> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<Tag> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.ArticleTags).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<Tag> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.TagId == id);
        }

        public IQueryable<Tag> Search(TagFilter filter)
        {
            var data = All();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                data = data.Where(p => p.ColorCode.Contains(filter.Keyword) || p.Name.Contains(filter.Keyword));
            }

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }

        public bool IsNameRepeat(string name, Guid? id = null)
        {
            var data = All().AsQueryable();
            if (!string.IsNullOrEmpty(name))
                data = data.Where(p => p.Name == name);
            if (id != null)
                data = data.Where(p => p.TagId != id);
            return data.Any();
        }

        public void CreateTag(string[] tagIds, string userId)
        {
            if (tagIds == null) return;

            for (int i = 0; i < tagIds.Length; i++)
            {
                if (!Guid.TryParse(tagIds[i], out _))
                {
                    var tag = new Tag
                    {
                        TagId = Guid.NewGuid(),
                        Name = tagIds[i],
                        DisplaySeq = GetMaxDisplaySeq()
                    };
                    tag.UserLog.CreateTime = DateTime.Now;
                    tag.UserLog.CreateUserId = userId;
                    Add(tag);
                    tagIds[i] = tag.TagId.ToString();
                }
            }
        }

        public async Task<List<TagDto>> QueryHotTag(string code, string tag)
        {
            // 改寫說明:
            // 原本最後 .Select(c => c.Tag) 投影過 entity 後再 Materialize,Include 失效 → ArticleTags == null,
            // 接著外面 .ArticleTags.Count() 就會丟 ArgumentNullException。
            // 解法:Count 在 IQueryable 階段就算完(EF 翻成 SQL COUNT),只把純數值資料拉回來。
            IQueryable<Tag> baseQuery = All();

            if (!string.IsNullOrEmpty(tag))
            {
                // 找出所有「跟此 tag 共現於同一篇已通過文章」的其他 tag
                baseQuery = baseQuery.Where(p =>
                    p.Name != tag
                    && p.ArticleTags.Any(at =>
                        at.Article.ArticleReviewType == ArticleReviewType.已通過
                        && at.Article.ArticleTags.Any(at2 => at2.Tag.Name == tag)));
            }
            else if (!string.IsNullOrEmpty(code))
            {
                baseQuery = baseQuery.Where(p => p.ArticleTags.Any(a =>
                    a.Article.ArticleReviewType == ArticleReviewType.已通過
                    && a.Article.ArticleCategoryArticles.Any(c => c.ArticleCategory.Code == code)));
            }
            else
            {
                baseQuery = baseQuery.Where(p => p.ArticleTags.Any(a =>
                    a.Article.ArticleReviewType == ArticleReviewType.已通過));
            }

            var raw = await baseQuery
                .Select(p => new
                {
                    p.TagId,
                    p.Name,
                    Count = p.ArticleTags.Count(a => a.Article.ArticleReviewType == ArticleReviewType.已通過)
                })
                .OrderByDescending(p => p.Count)
                .Take(10)
                .ToListAsync();

            return raw.Select(p => new TagDto
            {
                Id = p.TagId,
                Name = p.Name,
                Url = StrPath.Tag(p.Name),
                Count = p.Count
            }).ToList();
        }
    }
}
