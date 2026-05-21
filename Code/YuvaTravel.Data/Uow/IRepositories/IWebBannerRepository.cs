using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.WebBanners;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IWebBannerRepository : IBaseRepository<WebBanner>
    {
        IQueryable<WebBanner> All(bool isANT = true);
        Task<WebBanner> FindAsync(Guid? id, bool isANT = true);
        IQueryable<WebBanner> Search(WebBannerFilter filter);
        int GetMaxDisplaySeq();
    }
}
