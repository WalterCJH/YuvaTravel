using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Dtos.Home
{
    public class BannerDto
    {
        [Display(Name = "圖片路徑")]
        public string ImageUrl { get; set; }

        [Display(Name = "手機圖片路徑")]
        public string ImageMobileUrl { get; set; }

        [Display(Name = "圖片標題(title)")]
        public string ImageTitle { get; set; }

        [Display(Name = "圖片說明(alt)")]
        public string ImageAlt { get; set; }

        [Display(Name = "圖片點擊Url")]
        public string ImageClickUrl { get; set; }
    }
}
