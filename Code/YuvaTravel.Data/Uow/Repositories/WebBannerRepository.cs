using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.WebBanners;
using YuvaTravel.Data.Entities;
using YuvaTravel.Infrastructure.Helpers;

using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow.Repositories
{
    public class WebBannerRepository : BaseRepository<WebBanner>, IWebBannerRepository
    {
        public WebBannerRepository(YuvaTravelDbContext db) : base(db) { }

        public IQueryable<WebBanner> All(bool isANT = true)
        {
            var query = BaseAll().AsQueryable();
            if (isANT) query = query.AsNoTracking();
            return query;
        }

        public async Task<WebBanner> FindAsync(Guid? id, bool isANT = true)
        {
            return await All(isANT).FirstOrDefaultAsync(p => p.WebBannerId == id);
        }

        public IQueryable<WebBanner> Search(WebBannerFilter filter)
        {
            var data = All();

            if (!string.IsNullOrEmpty(filter.Keyword))
                data = data.Where(p => p.ImageTitle.Contains(filter.Keyword) || p.ImageAlt.Contains(filter.Keyword));

            var dateRangeDto = StringHelper.GetDateTimeRangeData(filter.RangeDate);

            if (dateRangeDto.StartTime != null && dateRangeDto.EndTime != null)
            {
                data = data.Where(p =>
                    p.BeginTime <= dateRangeDto.StartTime && p.EndTime >= dateRangeDto.StartTime ||
                    p.BeginTime <= dateRangeDto.EndTime && p.EndTime >= dateRangeDto.EndTime);
            }
            else if (dateRangeDto.StartTime != null)
            {
                data = data.Where(p => p.BeginTime <= dateRangeDto.StartTime && p.EndTime >= dateRangeDto.StartTime);
            }
            else if (dateRangeDto.EndTime != null)
            {
                data = data.Where(p => p.BeginTime <= dateRangeDto.EndTime && p.EndTime >= dateRangeDto.EndTime);
            }

            data = data.OrderBy($"{filter.SortBy} {filter.SortDirection}");
            return data;
        }

        public int GetMaxDisplaySeq()
        {
            var data = All().OrderByDescending(p => p.DisplaySeq).FirstOrDefault();
            return data != null ? data.DisplaySeq + IntNumber.DisplaySeqIncremental : IntNumber.DisplaySeqIncremental;
        }
    }
}
