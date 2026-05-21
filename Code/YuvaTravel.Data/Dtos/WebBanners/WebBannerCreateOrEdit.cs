using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YuvaTravel.Data.Dtos.WebBanners
{

    public class WebBannerCreateOrEdit
    {
        [Display(Name = "ID")]
        public Guid? WebBannerId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "開始時間")]
        public DateTime BeginTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "結束時間")]
        public DateTime EndTime { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "圖片標題(title)")]
        public string ImageTitle { get; set; }

        [MaxLength(100, ErrorMessage = StrText.OverLength)]
        [Display(Name = "圖片說明(alt)")]
        public string ImageAlt { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "圖片點擊Url")]
        public string ImageClickUrl { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        [Display(Name = "電腦圖片(W:1000、H:415)")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "手機圖片(W:660、H:660)")]
        public IFormFile ImageFileM { get; set; }

        [Display(Name = "電腦圖片路徑")]
        public string ImageUrl_ { get; set; }

        [Display(Name = "手機圖片路徑")]
        public string ImageMobileUrl_ { get; set; }

        public WebBannerCreateOrEdit()
        {
        }
    }
    
}
