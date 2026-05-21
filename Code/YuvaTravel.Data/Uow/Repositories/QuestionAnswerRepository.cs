using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.QuestionAnswers;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class QuestionAnswerRepository : BaseRepository<QuestionAnswer>, IQuestionAnswerRepository
    {
        public QuestionAnswerRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<QuestionAnswer> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<QuestionAnswer> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.QuestionAnswerId == id);
        }

        public IQueryable<QuestionAnswer> Search(QuestionAnswerFilter filter)
        {
            var data = All();

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Title.Contains(filter.Keyword) || p.Content.Contains(filter.Keyword) || p.Url.Contains(filter.Keyword));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }
    }
}
