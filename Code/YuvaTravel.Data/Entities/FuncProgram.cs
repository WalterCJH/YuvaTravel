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
    public class FuncProgram
    {
        [Key]
        [Required]
        [Column(TypeName = "VARCHAR")]
        [MaxLength(50)]
        public string FuncProgramId { get; set; }

        [NotMapped]
        [Display(Name = "代號")]
        public ProgramId ProgramId
        {
            get
            {
                ProgramId result;
                Enum.TryParse(FuncProgramId, out result);
                return result;
            }
            set { FuncProgramId = value.ToString(); }
        }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [MaxLength(30)]
        public string FuncGroupId { get; set; }
        [ForeignKey("FuncGroupId")]
        [Display(Name = "程式群組")]
        public virtual FuncGroup FuncGroup { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [MaxLength(200)]
        [Display(Name = "網址")]
        public string Url { get; set; }

        [Display(Name = "啟用新增")]
        public bool IsActiveCreate { get; set; }

        [Display(Name = "啟用修改")]
        public bool IsActiveEdit { get; set; }

        [Display(Name = "啟用刪除")]
        public bool IsActiveDelete { get; set; }

        [Display(Name = "啟用檢視")]
        public bool IsActiveDetails { get; set; }

        [Display(Name = "啟用匯入")]
        public bool IsActiveImport { get; set; }

        [Display(Name = "啟用匯出")]
        public bool IsActiveExport { get; set; }

        [Display(Name = "啟用功能")]
        public string IsActiveFuncIcon
        {
            get
            {
                string value = "";
                if (IsActiveCreate)
                {
                    value += "<i class=\"fas fa-plus\" title=\"新增\"></i>\r\n";
                }
                if (IsActiveEdit)
                {
                    value += "<i class=\"fas fa-pencil-alt\" title=\"修改\"></i>\r\n";
                }
                if (IsActiveDelete)
                {
                    value += "<i class=\"fas fa-trash\" title=\"刪除\"></i>\r\n";
                }
                if (IsActiveDetails)
                {
                    value += "<i class=\"fas fa-folder\" title=\"檢視\"></i>\r\n";
                }
                if (IsActiveImport)
                {
                    value += "<i class=\"fas fa-file-import\" title=\"匯入\"></i>\r\n";
                }
                if (IsActiveExport)
                {
                    value += "<i class=\"fas fa-file-export\" title=\"匯出\"></i>\r\n";
                }

                return value;
            }
        }


        [Display(Name = "變更新增")]
        public bool IsChangeCreate { get; set; }

        [Display(Name = "變更修改")]
        public bool IsChangeEdit { get; set; }

        [Display(Name = "變更刪除")]
        public bool IsChangeDelete { get; set; }

        [Display(Name = "變更檢視")]
        public bool IsChangeDetails { get; set; }

        [Display(Name = "變更匯入")]
        public bool IsChangeImport { get; set; }

        [Display(Name = "變更匯出")]
        public bool IsChangeExport { get; set; }

        [Display(Name = "變更功能")]
        public string IsChangeFuncIcon
        {
            get
            {
                string value = "";
                if (IsChangeCreate)
                {
                    value += "<i class=\"fas fa-plus\" title=\"新增\"></i>\r\n";
                }
                if (IsChangeEdit)
                {
                    value += "<i class=\"fas fa-pencil-alt\" title=\"修改\"></i>\r\n";
                }
                if (IsChangeDelete)
                {
                    value += "<i class=\"fas fa-trash\" title=\"刪除\"></i>\r\n";
                }
                if (IsChangeDetails)
                {
                    value += "<i class=\"fas fa-folder\" title=\"檢視\"></i>\r\n";
                }
                if (IsChangeImport)
                {
                    value += "<i class=\"fas fa-file-import\" title=\"匯入\"></i>\r\n";
                }
                if (IsChangeExport)
                {
                    value += "<i class=\"fas fa-file-export\" title=\"匯出\"></i>\r\n";
                }

                return value;
            }
        }

        [Display(Name = "權限等級")]
        public AuthorizeLevel AuthorizeLevel { get; set; }

        [Required]
        [Display(Name = "顯示順序")]
        public int DisplaySeq { get; set; }

        [Display(Name = "系統管理員")]
        public bool OnlyAdmin { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<UserGroupFuncProgram> UserGroupFuncPrograms { get; set; }

        public FuncProgram()
        {
            UserLog = new UserLog();
            UserGroupFuncPrograms = new HashSet<UserGroupFuncProgram>();
        }
    }
}
