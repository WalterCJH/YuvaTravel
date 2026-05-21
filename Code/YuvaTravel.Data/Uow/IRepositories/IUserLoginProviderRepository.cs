using System.Linq;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IUserLoginProviderRepository : IBaseRepository<UserLoginProvider>
    {
        IQueryable<UserLoginProvider> All(bool isANT = true);
        IQueryable<UserLoginProvider> IncludeAll(bool isANT = true);
    }
}
