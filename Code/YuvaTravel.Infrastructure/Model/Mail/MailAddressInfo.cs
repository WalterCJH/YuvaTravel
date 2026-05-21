using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Infrastructure.Model.Mail
{
    public class MailAddressInfo
    {
        public MailAddressInfo(string Address, string DisplayName = null)
        {
            this.Address = Address;
            this.DisplayName = DisplayName;
        }

        /// <summary>
        /// 電子郵件地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string DisplayName { get; set; }
    }
}
