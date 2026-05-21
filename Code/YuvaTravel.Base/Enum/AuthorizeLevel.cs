using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Base.Enum
{
    public enum AuthorizeLevel : int
    {
        [Display(Name = "一般管理員")]
        一般管理員 = 10,
        [Display(Name = "後台管理員")]
        後台管理員 = 88888,
        [Display(Name = "系統管理員")]
        系統管理員 = 99999,
    }
}
