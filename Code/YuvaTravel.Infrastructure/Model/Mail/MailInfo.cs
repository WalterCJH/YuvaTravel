using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Infrastructure.Model.Mail
{
    public class MailInfo
    {
        /// <summary>
        /// 收件者
        /// </summary>
        public List<MailAddressInfo> Receivers { get; set; } = new List<MailAddressInfo>();
        /// <summary>
        /// 副本
        /// </summary>
        public List<MailAddressInfo> CCs { get; set; } = new List<MailAddressInfo>();
        /// <summary>
        /// 密件副本
        /// </summary>
        public List<MailAddressInfo> BCCs { get; set; } = new List<MailAddressInfo>();
        /// <summary>
        /// 主旨
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// HTML格式
        /// </summary>
        public bool IsBodyHtml { get; set; }
    }
}
