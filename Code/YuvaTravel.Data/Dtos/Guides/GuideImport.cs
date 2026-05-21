using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using YuvaTravel.Base.Constants;
using YuvaTravel.Infrastructure.ActionFilters;

namespace YuvaTravel.Data.Dtos.Guides
{
    public class GuideImport
    {
        [ExcelExt(Allow = ".xls,.xlsx", ErrorMessage = StrText.OnlyExcel)]
        [Required(ErrorMessage = StrText.FileSelected)]
        [Display(Name = "上傳檔案")]
        public IFormFile File { get; set; }
    }
}
