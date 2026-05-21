using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IUserForgetPasswordRepository : IBaseRepository<UserForgetPassword>
    {
        IQueryable<UserForgetPassword> All(bool isANT = true);
        UserForgetPassword Find(Guid id);
        Task<UserForgetPassword> FindAsync(Guid id, bool isANT = true);
    }
}
