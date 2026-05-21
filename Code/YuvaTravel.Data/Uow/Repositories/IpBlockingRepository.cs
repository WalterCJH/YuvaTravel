using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.IpBlockings;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class IpBlockingRepository : BaseRepository<IpBlocking>, IIpBlockingRepository
    {
        public IpBlockingRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<IpBlocking> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<IpBlocking> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.IpBlockingId == id);
        }
        
        public IpBlocking FindIp(string ip, bool isANT = true)
        {
            return All(isANT).FirstOrDefault(p => p.IpAddress == ip);
        }

        public bool IsIpExist(string ip)
        {
            return All().Any(p => p.IsActive && p.IpAddress == ip);
        }

        public IQueryable<IpBlocking> Search(IpBlockingFilter filter)
        {
            var data = All();

            if (filter.IsActive == WhetherType.否)
                data = data.Where(p => p.IsActive == false);
            else if (filter.IsActive == WhetherType.是)
                data = data.Where(p => p.IsActive == true);

            if (!string.IsNullOrEmpty(filter.IpAddress))
                data = data.Where(p => p.IpAddress.Contains(filter.IpAddress));

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.Memo.Contains(filter.Keyword));

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public bool IsIpRepeat(string ip, Guid? id)
        {
            var data = All();
            if (!string.IsNullOrEmpty(ip))
                data = data.Where(p => p.IpAddress == ip);
            if (id != null)
                data = data.Where(p => p.IpBlockingId != id);
            return data.Any();
        }

        /// <summary>判斷是否為不完整 IP</summary>
        public bool IsIncompleteIP(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return false;

            if (ip.Contains(":"))
            {
                var tmpIPs = ip.Split(':');
                var num = tmpIPs.Count(t => !string.IsNullOrWhiteSpace(t));
                if (num < 8) return true;
            }
            else
            {
                var tmpIPs = ip.Split('.');
                var num = tmpIPs.Count(t => !string.IsNullOrWhiteSpace(t));
                if (num < 4) return true;
            }
            return false;
        }

        public string IncompleteIPAddDot(string ip)
        {
            if (!ip.EndsWith(".")) ip += ".";
            return ip;
        }

        public List<string> IncompleteIPaddressList()
        {
            return All().Where(p => p.IsIncompleteIP).Select(p => p.IpAddress).ToList();
        }
    }
}
