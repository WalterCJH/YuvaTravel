using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.RecruitAgents;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IRecruitAgentRepository : IBaseRepository<RecruitAgent>
    {
        IQueryable<RecruitAgent> All(bool isANT = true);
        Task<RecruitAgent> FindAsync(Guid? id, bool isANT = true);
        IQueryable<RecruitAgent> Search(RecruitAgentFilter filter);
    }
}
