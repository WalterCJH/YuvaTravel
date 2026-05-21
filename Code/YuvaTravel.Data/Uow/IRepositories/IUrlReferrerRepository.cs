using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IUrlReferrerRepository : IBaseRepository<UrlReferrer>
    {
        IQueryable<UrlReferrer> All(bool isANT = true);
        UrlReferrer Find(Guid? id, bool isANT = true);
        Task<UrlReferrer> FindAsync(Guid? id, bool isANT = true);
    }
}
