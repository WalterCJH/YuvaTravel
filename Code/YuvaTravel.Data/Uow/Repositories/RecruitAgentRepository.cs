using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Dtos.RecruitAgents;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class RecruitAgentRepository : BaseRepository<RecruitAgent>, IRecruitAgentRepository
    {
        public RecruitAgentRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<RecruitAgent> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<RecruitAgent> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.RecruitAgentId == id);
        }

        public IQueryable<RecruitAgent> Search(RecruitAgentFilter filter)
        {
            var data = All();

            data = data.Where(p => p.IsContact == filter.IsContact);

            if (!string.IsNullOrEmpty(filter.Nick))
                data = data.Where(p => p.Nick.Contains(filter.Nick));

            if (!string.IsNullOrEmpty(filter.Phone))
                data = data.Where(p => p.Phone.Contains(filter.Phone));

            if (!string.IsNullOrEmpty(filter.TelegramID))
                data = data.Where(p => p.TelegramID.Contains(filter.TelegramID));

            if (!string.IsNullOrEmpty(filter.LineID))
                data = data.Where(p => p.LineID.Contains(filter.LineID));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }
    }
}
