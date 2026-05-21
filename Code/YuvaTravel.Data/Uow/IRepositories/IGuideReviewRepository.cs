using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Guides;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IGuideReviewRepository : IBaseRepository<GuideReview>
    {
        IQueryable<GuideReview> All(bool isANT = true);
        IQueryable<GuideReview> IncludeAll(bool isANT = true);
        Task<GuideReview> FindAsync(Guid? id, bool isANT = true);
        IQueryable<GuideReview> Search(GuideReviewFilter filter);
        int GetMaxDisplaySeq(Guid? guideId);
    }
}
