using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.Users
{

    [MetadataType(typeof(UserDto))]
    public class UserDetailsOrDelete : UserDto
    {
        [Display(Name = "使用者群組")]
        public string UserGroupName { get; set; }

        [Display(Name = "稱謂")]
        public string Salutation { get; set; }

        public UserDetailsOrDelete()
        {
        }
    }
}
