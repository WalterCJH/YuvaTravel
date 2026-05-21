using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Dtos.ArticleComments;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class ArticleCommentRepository : BaseRepository<ArticleComment>, IArticleCommentRepository
    {
        public ArticleCommentRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<ArticleComment> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public IQueryable<ArticleComment> IncludeAll(bool isANT = true)
        {
            var query = BaseAll().Include(p => p.Article).AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<ArticleComment> FindAsync(Guid? id, bool isANT = true)
        {
            return await IncludeAll(isANT).FirstOrDefaultAsync(p => p.ArticleCommentId == id);
        }

        public List<ArticleCommentCreateOrEdit> FindFromArticleId(Guid id, string userId)
        {
            var dto = All()
                .Where(a => a.ArticleId == id)
                .OrderByDescending(p => p.CommentTime)
                .Select(p => new ArticleCommentCreateOrEdit
                {
                    ArticleCommentId = p.ArticleCommentId,
                    ArticleId = p.ArticleId,
                    CommentContent = p.CommentContent,
                    CommentTime = p.CommentTime,
                    CommentUserId = p.CommentUserId,
                    ReplyContent = p.ReplyContent,
                    ReplyTime = p.ReplyTime,
                    ReplyUserId = p.ReplyUserId
                }).ToList();

            for (int i = 0; i < dto.Count; i++)
                dto[i].UserId = userId;

            return dto;
        }
    }
}
