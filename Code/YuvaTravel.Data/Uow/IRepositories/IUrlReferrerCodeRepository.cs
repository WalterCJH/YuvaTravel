using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.UrlReferrerCodes;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IUrlReferrerCodeRepository : IBaseRepository<UrlReferrerCode>
    {
        IQueryable<UrlReferrerCode> All(bool isANT = true);
        UrlReferrerCode Find(Guid? id, bool isANT = true);
        Task<UrlReferrerCode> FindAsync(Guid? id, bool isANT = true);
        IQueryable<UrlReferrerCode> Search(UrlReferrerCodeFilter filter);
        bool IsCodeRepeat(string code, Guid? id);
    }
}
