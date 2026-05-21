using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IWebConfigRepository : IBaseRepository<WebConfig>
    {
        IQueryable<WebConfig> All(bool isANT = true);
        WebConfig Find(bool isANT = true);
        Task<WebConfig> FindAsync(bool isANT = true);
    }
}
