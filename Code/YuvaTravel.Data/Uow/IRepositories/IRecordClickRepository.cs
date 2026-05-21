using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IRecordClickRepository : IBaseRepository<RecordClick>
    {
        IQueryable<RecordClick> All(bool isANT = true);
        RecordClick Find(Guid? id);
        Task<RecordClick> FindAsync(Guid? id, bool isANT = true);
    }
}
