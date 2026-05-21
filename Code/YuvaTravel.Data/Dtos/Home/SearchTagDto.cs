using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Home
{
    public class SearchTagDto
    {
        [Display(Name = "標籤名稱")]
        public string Id { get; set; }

        [Display(Name = "標籤敘述")]
        public string Description { get; set; }

        [Display(Name = "分類")]
        public string Category { get; set; }

        [Display(Name = "搜尋結果數量")]
        public int TotalCount { get; set; }

        //[RegularExpression(@"(DisplaySeq)")]
        //public string SortBy { get; set; } = "DisplaySeq";

        //[RegularExpression(@"(ASC|DESC)")]
        //public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

        [Display(Name = "文章")]
        public IEnumerable<BaseArticleDto> Articles { get; set; }

        [Display(Name = "相關文章類別")]
        public List<SearchCategoryDto> Categories { get; set; }

        public SearchTagDto()
        {
            Articles = new List<BaseArticleDto>();
            Categories = new List<SearchCategoryDto>();
        }
    }
}
