using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using YuvaTravel.Base.Constants;
using YuvaTravel.Data.Dtos.Regions;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Data.Dtos.Guides
{
    public class GuideCreateOrEdit : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var uow = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
            if (uow != null && !string.IsNullOrWhiteSpace(Code) && uow.GuideRepo.IsCodeRepeat(Code, GuideId))
            {
                yield return new ValidationResult(StrText.CodeNotRepeat, new[] { nameof(Code) });
            }
        }

        [Display(Name = "ID")]
        public Guid? GuideId { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "本月精選")]
        public bool IsFeatured { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "代碼 (網址)")]
        public string Code { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "中文名")]
        public string Name { get; set; }

        [MaxLength(80, ErrorMessage = StrText.OverLength)]
        [Display(Name = "英文名")]
        public string NameEn { get; set; }

        [Display(Name = "在地年數")]
        public int YearsOfExperience { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "可用語言 (例 JP · EN)")]
        public string Languages { get; set; }

        [MaxLength(200, ErrorMessage = StrText.OverLength)]
        [Display(Name = "短簡介 (卡片用)")]
        public string Bio { get; set; }
                
        [Display(Name = "完整介紹")]
        public string LongBio { get; set; }

        [MaxLength(300, ErrorMessage = StrText.OverLength)]
        [Display(Name = "個人語錄")]
        public string QuoteText { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "語錄署名")]
        public string QuoteBy { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "肖像圖 URL")]
        public string PortraitUrl { get; set; }

        [Display(Name = "肖像圖檔案")]
        public IFormFile PortraitFile { get; set; }

        [Display(Name = "肖像圖 Base64")]
        public string PortraitFileBase64 { get; set; }

        [Display(Name = "肖像圖檔名")]
        public string PortraitFileName { get; set; }

        [Display(Name = "肖像圖類型")]
        public string PortraitFileType { get; set; }

        [MaxLength(500, ErrorMessage = StrText.OverLength)]
        [Display(Name = "封面圖 URL")]
        public string CoverUrl { get; set; }

        [Display(Name = "封面圖檔案")]
        public IFormFile CoverFile { get; set; }

        [Display(Name = "封面圖 Base64")]
        public string CoverFileBase64 { get; set; }

        [Display(Name = "封面圖檔名")]
        public string CoverFileName { get; set; }

        [Display(Name = "封面圖類型")]
        public string CoverFileType { get; set; }

        [Range(0, 5)]
        [Display(Name = "評分 (0–5)")]
        public decimal Rating { get; set; }

        [Display(Name = "評論數")]
        public int ReviewCount { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "服務形式 (例 半日 / 全日)")]
        public string ServiceFormat { get; set; }

        [Display(Name = "適合人數下限")]
        public int? MinPeople { get; set; }

        [Display(Name = "適合人數上限")]
        public int? MaxPeople { get; set; }

        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "回覆時間 (例 24 小時內)")]
        public string ResponseTime { get; set; }

        [Display(Name = "起價 (NT$)")]
        public int? StartingPrice { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        [Display(Name = "嚮導標籤")]
        public string[] TagIds { get; set; }

        [Display(Name = "全選地區")]
        public bool CheckAllRegions { get; set; }

        public List<RegionPickDto> Regions { get; set; } = new List<RegionPickDto>();

        public void SettingRegions(List<Region> allRegions, List<Guid> activeRegionIdList = null)
        {
            foreach (var region in allRegions.OrderBy(p => p.DisplaySeq))
            {
                var pick = new RegionPickDto
                {
                    RegionId = region.RegionId,
                    Name = region.Name,
                    IsActive = activeRegionIdList != null && activeRegionIdList.Contains(region.RegionId)
                };
                Regions.Add(pick);
            }
        }
    }
}
