using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Subscribers;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class SubscriberRepository : BaseRepository<Subscriber>, ISubscriberRepository
    {
        public SubscriberRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<Subscriber> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<Subscriber> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.SubscriberId == id);
        }

        public async Task<Subscriber> FindByEmailAsync(string email, bool isANT = true)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var normalized = email.Trim().ToLowerInvariant();
            return await All(isANT).FirstOrDefaultAsync(p => p.Email == normalized);
        }

        /// <summary>找出該 email 目前「啟用中」的訂閱 (用來判斷是否已訂閱)</summary>
        public async Task<Subscriber> FindActiveByEmailAsync(string email, bool isANT = true)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var normalized = email.Trim().ToLowerInvariant();
            return await All(isANT)
                .Where(p => p.Email == normalized && p.IsActive)
                .OrderByDescending(p => p.SubscribeTime)
                .FirstOrDefaultAsync();
        }

        public async Task<Subscriber> FindByTokenAsync(Guid token, bool isANT = true)
        {
            if (token == Guid.Empty) return null;
            return await All(isANT).FirstOrDefaultAsync(p => p.UnsubscribeToken == token);
        }

        public IQueryable<Subscriber> Search(SubscriberFilter filter)
        {
            var data = All();

            if (filter.IsActive == WhetherType.是)
                data = data.Where(p => p.IsActive);
            else if (filter.IsActive == WhetherType.否)
                data = data.Where(p => !p.IsActive);

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Email.Contains(filter.Keyword) || p.SourcePath.Contains(filter.Keyword) || p.IpAddress.Contains(filter.Keyword));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }
    }
}
