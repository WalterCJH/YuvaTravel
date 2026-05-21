using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using YuvaTravel.Base.Constants;
using YuvaTravel.Infrastructure.ActionFilters;

namespace YuvaTravel.Data.Dtos.HotKeywords
{
    public class HotKeywordImport
    {
        [ExcelExt(Allow = ".xls,.xlsx", ErrorMessage = StrText.OnlyExcel)]
        [Required(ErrorMessage = StrText.FileSelected)]
        [Display(Name = "上傳檔案")]
        public IFormFile File { get; set; }
    }
}
