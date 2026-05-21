using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Columns;
using YuvaTravel.Infrastructure.Helpers;

namespace YuvaTravel.Data.Entities
{
    public class User
    {
        [Key]
        public Guid UserGuid { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [MaxLength(30)]
        [Display(Name = "帳號")]
        public string UserId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(30)]
        [Display(Name = "使用者群組")]
        public string UserGroupId { get; set; }
        [ForeignKey("UserGroupId")]
        public virtual UserGroup UserGroup { get; set; }

        [Display(Name = "變更密碼")]
        public bool LoginResetPassword { get; set; }

        [Display(Name = "啟用")]
        public bool IsActive { get; set; }

        [MaxLength(30)]
        [Display(Name = "姓")]
        public string LastName { get; set; }

        [MaxLength(30)]
        [Display(Name = "名")]
        public string FirstName { get; set; }

        [MaxLength(2)]
        [Display(Name = "稱謂")]
        public string Salutation { get; set; }

        [MaxLength(10)]
        [Display(Name = "手機")]
        public string Mobile { get; set; }

        //[MaxLength(4)]
        //[Display(Name = "電話區碼")]
        //public string PhoneAreaNo { get; set; }

        //[MaxLength(20)]
        //[Display(Name = "電話號碼")]
        //public string PhoneNo { get; set; }

        //[MaxLength(10)]
        //[Display(Name = "分機")]
        //public string PhoneExt { get; set; }

        [Column("Phone")]
        [Display(Name = "電話")]
        public Phone Phone { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [MaxLength(100)]
        [Display(Name = "Email (登入帳號)")]
        public string Email { get; set; }

        [Column(TypeName = "VARCHAR")]
        [MaxLength(128)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        public UserLog UserLog { get; set; }

        [Display(Name = "姓名")]
        public string Name
        {
            get
            {
                if (RegularHelper.IsEng(LastName + FirstName))
                    return $"{LastName}, {FirstName}";
                else
                    return $"{LastName}{FirstName}";
            }
        }

        public virtual ICollection<UserForgetPassword> UserForgetPasswords { get; set; }

        [Display(Name = "第三方登入")]
        public virtual ICollection<UserLoginProvider> UserLoginProviders { get; set; }

        //public SelectList SalutationList
        //{
        //    get
        //    {
        //        List<string> list = new List<string>
        //        {
        //            "男",
        //            "女"
        //        };
        //        return new SelectList(list, Salutation);
        //    }
        //}


        public User()
        {
            UserLog = new UserLog();
            Phone = new Phone();
        }
    }
}
