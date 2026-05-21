using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.HotKeywords;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IHotKeywordRepository : IBaseRepository<HotKeyword>
    {
        IQueryable<HotKeyword> All(bool isANT = true);
        Task<HotKeyword> FindAsync(Guid? id, bool isANT = true);
        IQueryable<HotKeyword> Search(HotKeywordFilter filter);
        int GetMaxDisplaySeq();
        bool IsNameRepeat(string name, Guid? id = null);
    }
}
