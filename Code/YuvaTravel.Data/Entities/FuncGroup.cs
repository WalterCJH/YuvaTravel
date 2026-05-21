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
    public class FuncGroup
    {
        [Key]
        [Required]
        [MaxLength(30)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "代號")]
        public string FuncGroupId { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Required]
        [MaxLength(30)]
        [Column(TypeName = "VARCHAR")]
        [Display(Name = "icon圖示")]
        public string IconClass { get; set; }

        [Required]
        [Display(Name = "顯示順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<FuncProgram> FuncPrograms { get; set; }

        public FuncGroup()
        {
            UserLog = new UserLog();
        }
    }
}
