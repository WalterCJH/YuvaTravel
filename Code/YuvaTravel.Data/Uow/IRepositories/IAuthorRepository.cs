using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Authors;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IAuthorRepository : IBaseRepository<Author>
    {
        IQueryable<Author> IncludeAll(bool isANT = true);
        IQueryable<Author> QueryActive(bool isANT = true);
        Task<Author> FindAsync(Guid? id, bool isANT = true);
        IQueryable<Author> Search(AuthorFilter filter);
        int GetMaxDisplaySeq();
    }
}
