using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Home
{
    public class SearchCategoryDto
    {
        [Display(Name = "分類名稱")]
        public string Name { get; set; }

        [Display(Name = "分類數量")]
        public int Count { get; set; }

        [Display(Name = "分類連結")]
        public string Url { get; set; }

        [Display(Name = "已選擇")]
        public string Active { get; set; }
    }
}
