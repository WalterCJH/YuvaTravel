using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class UserGroupFuncProgram
    {
        [Key]
        [Required]
        [Display(Name = "ID")]
        public Guid UserGroupFuncProgramId { get; set; }

        [MaxLength(30)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "使用者群組")]
        public string UserGroupId { get; set; }
        [ForeignKey("UserGroupId")]
        public virtual UserGroup UserGroup { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "功能程式")]
        public string FuncProgramId { get; set; }
        [ForeignKey("FuncProgramId")]
        public virtual FuncProgram FuncProgram { get; set; }

        [Display(Name = "新增")]
        public bool IsCreate { get; set; }

        [Display(Name = "修改")]
        public bool IsEdit { get; set; }

        [Display(Name = "刪除")]
        public bool IsDelete { get; set; }

        [Display(Name = "檢視")]
        public bool IsDetails { get; set; }

        [Display(Name = "匯入")]
        public bool IsImport { get; set; }

        [Display(Name = "匯出")]
        public bool IsExport { get; set; }

        public UserLog UserLog { get; set; }


        public UserGroupFuncProgram()
        {
            UserLog = new UserLog();
        }
    }
}
