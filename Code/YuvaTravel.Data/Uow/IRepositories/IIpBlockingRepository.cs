using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.IpBlockings;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IIpBlockingRepository : IBaseRepository<IpBlocking>
    {
        IQueryable<IpBlocking> All(bool isANT = true);
        Task<IpBlocking> FindAsync(Guid? id, bool isANT = true);
        IpBlocking FindIp(string ip, bool isANT = true);
        bool IsIpExist(string ip);
        IQueryable<IpBlocking> Search(IpBlockingFilter filter);
        bool IsIpRepeat(string ip, Guid? id);
        bool IsIncompleteIP(string ip);
        string IncompleteIPAddDot(string ip);
        List<string> IncompleteIPaddressList();
    }
}
