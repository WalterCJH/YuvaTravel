using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class UserGroup
    {
        [Key]
        [Required]
        [MaxLength(30)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "代號")]
        public string UserGroupId { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Display(Name = "權限等級")]
        public AuthorizeLevel AuthorizeLevel { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<UserGroupFuncProgram> UserGroupFuncPrograms { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public UserGroup()
        {
            UserLog = new UserLog();
            UserGroupFuncPrograms = new HashSet<UserGroupFuncProgram>();
            Users = new HashSet<User>();
        }
    }
}
