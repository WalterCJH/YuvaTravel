using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Dtos.Tags;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Uow.IRepositories
{
    public interface ITagRepository : IBaseRepository<Tag>
    {
        IQueryable<Tag> All(bool isANT = true);
        IQueryable<Tag> IncludeAll(bool isANT = true);
        Task<Tag> FindAsync(Guid? id, bool isANT = true);
        IQueryable<Tag> Search(TagFilter filter);
        int GetMaxDisplaySeq();
        bool IsNameRepeat(string name, Guid? id = null);

        /// <summary>把 tagIds 內非 GUID 字串(等於新建立的 tag 名)轉成新 Tag entity 加入 context,並把陣列內容替換成新的 TagId 字串。</summary>
        void CreateTag(string[] tagIds, string userId);

        /// <summary>熱門標籤查詢。code: 文章類別代碼;tag: 當前主標籤名稱;兩者皆空則取全站熱門。</summary>
        Task<List<TagDto>> QueryHotTag(string code, string tag);
    }
}
