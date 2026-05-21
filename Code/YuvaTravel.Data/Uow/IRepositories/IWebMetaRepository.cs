using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.WebMetas;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IWebMetaRepository : IBaseRepository<WebMeta>
    {
        IQueryable<WebMeta> All(bool isANT = true);
        Task<WebMeta> FindAsync(Guid? id, bool isANT = true);
        IQueryable<WebMeta> Search(WebMetaFilter filter);
        int GetMaxDisplaySeq();
        bool IsUrlRepeat(string url, Guid? id);
    }
}
