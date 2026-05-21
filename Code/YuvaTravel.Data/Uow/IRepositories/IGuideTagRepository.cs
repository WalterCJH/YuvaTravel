using System;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface IGuideTagRepository : IBaseRepository<GuideTag>
    {
        IQueryable<GuideTag> All(bool isANT = true);
        IQueryable<GuideTag> IncludeAll(bool isANT = true);
        Task<GuideTag> FindAsync(Guid? id, bool isANT = true);

        void UpdateGuideTagFromGuide(Guid guideId, string[] tagIds);
        string[] QueryTagFromGuide(Guid guideId);
    }
}
