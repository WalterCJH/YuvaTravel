using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Home
{
    public class ArticleCategoryDto
    {
        [Display(Name = "文章類別英文名稱")]
        public string Id { get; set; }

        [RegularExpression(@"(DisplaySeq)")]
        public string SortBy { get; set; } = "DisplaySeq";

        [RegularExpression(@"(ASC|DESC)")]
        public string SortDirection { get; set; } = "ASC";

        [Display(Name = "頁數")]
        public int PageNo { get; set; } = 1;

        [Display(Name = "文章")]
        public List<BaseArticleDto> Articles { get; set; }

        [Display(Name = "文章類別名稱")]
        public string CategoryName { get; set; }

        [Display(Name = "搜尋文章關鍵字")]
        public string S { get; set; }

        [Display(Name = "搜尋文章標籤關鍵字")]
        public string T { get; set; }

        public ArticleCategoryDto()
        {
            Articles = new List<BaseArticleDto>();
        }
    }
}
