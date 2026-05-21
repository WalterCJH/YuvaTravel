using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.QuestionAnswers;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IQuestionAnswerRepository : IBaseRepository<QuestionAnswer>
    {
        IQueryable<QuestionAnswer> All(bool isANT = true);
        Task<QuestionAnswer> FindAsync(Guid? id, bool isANT = true);
        IQueryable<QuestionAnswer> Search(QuestionAnswerFilter filter);
        int GetMaxDisplaySeq();
    }
}
