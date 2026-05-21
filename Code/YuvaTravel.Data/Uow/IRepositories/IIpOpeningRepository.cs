using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.IpOpenings;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IIpOpeningRepository : IBaseRepository<IpOpening>
    {
        IQueryable<IpOpening> All(bool isANT = true);
        Task<IpOpening> FindAsync(Guid? id, bool isANT = true);
        IpOpening FindIp(string ip, bool isANT = true);
        bool IsIpExist(string ip);
        IQueryable<IpOpening> Search(IpOpeningFilter filter);
        bool IsIpRepeat(string ip, Guid? id);
        bool IsIncompleteIP(string ip);
        string IncompleteIPAddDot(string ip);
        List<string> IncompleteIPaddressList();
    }
}
