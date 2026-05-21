using YuvaTravel.Base.Constants;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.FuncGroups
{

    public class FuncGroupCreateOrEdit
    {
        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "代號")]
        public string FuncGroupId { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(50, ErrorMessage = StrText.OverLength)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "icon圖示")]
        public string IconClass { get; set; }

        [Required(ErrorMessage = StrText.Required)]
        [Display(Name = "顯示順序")]
        public int DisplaySeq { get; set; }

        public FuncGroupCreateOrEdit()
        {
        }
    }
}
