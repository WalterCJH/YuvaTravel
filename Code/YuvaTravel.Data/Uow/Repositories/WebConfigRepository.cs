using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class WebConfigRepository : BaseRepository<WebConfig>, IWebConfigRepository
    {
        public WebConfigRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<WebConfig> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public WebConfig Find(bool isANT = true) => All(isANT).FirstOrDefault();

        public async Task<WebConfig> FindAsync(bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync();
        }
    }
}
