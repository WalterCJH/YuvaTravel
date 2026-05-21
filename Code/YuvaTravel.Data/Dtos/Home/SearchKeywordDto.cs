using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Home
{
    public class SearchKeywordDto
    {
        [Required(ErrorMessage = "請輸入關鍵字")]
        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }

        [Display(Name = "分類")]
        public string Category { get; set; }

        [Display(Name = "搜尋結果")]
        public string Result { get; set; }

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

        public SearchKeywordDto()
        {
            Articles = new List<BaseArticleDto>();
            Categories = new List<SearchCategoryDto>();
        }
    }
}
