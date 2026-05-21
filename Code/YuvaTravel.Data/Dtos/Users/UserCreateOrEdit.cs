using System.ComponentModel.DataAnnotations;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Data.Dtos.Users
{
    [MetadataType(typeof(UserDto))]
    public class UserCreateOrEdit : UserDto
    {
        [Required(ErrorMessage = StrText.RequireSelectd)]
        [MaxLength(30, ErrorMessage = StrText.OverLength)]
        [Display(Name = "使用者群組")]
        public string UserGroupId { get; set; }

        [MaxLength(2, ErrorMessage = StrText.OverLength)]
        [Display(Name = "稱謂")]
        public string Salutation { get; set; }

        public UserCreateOrEdit()
        {
        }
    }
}
