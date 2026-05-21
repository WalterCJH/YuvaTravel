using YuvaTravel.Data.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Entities
{
    public class Tag
    {
        [Key]
        [Display(Name = "ID")]
        public Guid TagId { get; set; }

        [Column(TypeName = "CHAR")]
        [MaxLength(7)]
        [Display(Name = "顏色代碼")]
        public string ColorCode { get; set; }

        [MaxLength(20)]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [MaxLength(100)]
        [Display(Name = "敘述")]
        public string Description { get; set; }

        public string ShowDesc
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return "";
                int length = Description.Length;
                if (length > 15)
                {
                    length = 15;
                }
                return Description?.Substring(0, length);
            }
        }

        [Display(Name = "順序")]
        public int DisplaySeq { get; set; }

        public UserLog UserLog { get; set; }

        public virtual ICollection<ArticleTag> ArticleTags { get; set; }

        public Tag()
        {
            UserLog = new UserLog();
        }
    }
}
