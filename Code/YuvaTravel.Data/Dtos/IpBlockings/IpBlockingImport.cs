using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using YuvaTravel.Base.Constants;
using YuvaTravel.Infrastructure.ActionFilters;

namespace YuvaTravel.Data.Dtos.IpBlockings
{
    public class IpBlockingImport
    {
        [ExcelExt(Allow = ".xls,.xlsx", ErrorMessage = StrText.OnlyExcel)]
        [Required(ErrorMessage = StrText.FileSelected)]
        [Display(Name = "上傳檔案")]
        public IFormFile File { get; set; }

        [Display(Name = "覆蓋全部資料")]
        public bool IsReplaceAllData { get; set; }
    }
}
