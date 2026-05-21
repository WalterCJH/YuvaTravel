using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Subscribers;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface ISubscriberRepository : IBaseRepository<Subscriber>
    {
        IQueryable<Subscriber> All(bool isANT = true);
        Task<Subscriber> FindAsync(Guid? id, bool isANT = true);
        Task<Subscriber> FindByEmailAsync(string email, bool isANT = true);
        Task<Subscriber> FindActiveByEmailAsync(string email, bool isANT = true);
        Task<Subscriber> FindByTokenAsync(Guid token, bool isANT = true);
        IQueryable<Subscriber> Search(SubscriberFilter filter);
    }
}
