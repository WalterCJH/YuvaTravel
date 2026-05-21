using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Data.Entities;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class RecordClickRepository : BaseRepository<RecordClick>, IRecordClickRepository
    {
        public RecordClickRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<RecordClick> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public RecordClick Find(Guid? id) => BaseAll().FirstOrDefault(p => p.RecordClickId == id);

        public async Task<RecordClick> FindAsync(Guid? id, bool isANT = true)
            => await All(isANT).FirstOrDefaultAsync(p => p.RecordClickId == id);
    }
}
