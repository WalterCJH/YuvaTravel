using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace YuvaTravel.Data.Columns
{
    [Owned]
    public class Phone
    {
        //[Column("PhoneAreaNo")]
        [MaxLength(4)]
        [Display(Name = "電話區碼")]
        public string AreaNo { get; set; }

        //[Column("PhoneNo")]
        [MaxLength(20)]
        [Display(Name = "電話號碼")]
        [Phone]
        public string No { get; set; }

        //[Column("PhoneExt")]
        [MaxLength(10)]
        [Display(Name = "分機")]
        public string Ext { get; set; }

        [Display(Name = "完整電話")]
        public string FullNo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(No)) return string.Empty;
                StringBuilder stringBuilder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(AreaNo)) stringBuilder.AppendFormat("({0})", AreaNo);
                stringBuilder.Append(No);
                if (!string.IsNullOrWhiteSpace(Ext)) stringBuilder.AppendFormat("#{0}", Ext);
                return stringBuilder.ToString();
            }
        }

    }
}
