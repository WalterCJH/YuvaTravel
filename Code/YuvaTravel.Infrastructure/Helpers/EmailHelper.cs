using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Infrastructure.Model.Mail;

namespace YuvaTravel.Infrastructure.Helpers
{
    public class EmailHelper
    {
        private readonly EmailOptions _options;

        public EmailHelper(SystemConfig systemConfig, EmailOptions options)
        {
            SystemConfig = systemConfig;
            _options = options;
        }

        public SystemConfig SystemConfig { get; set; }

        /// <summary>測試人員 Email(開發模式只寄給這個人)</summary>
        private bool IsTestEmail(string email) => email == _options.DeveloperEmail;

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = _options.Host,
                Port = (SystemConfig == SystemConfig.Production) ? _options.PortProduction : _options.PortTesting,
                EnableSsl = _options.EnableSsl,
                Credentials = new System.Net.NetworkCredential(_options.SenderEmail, _options.Password)
            };
        }

        /// <summary>
        /// Develop:自動加主旨[開發]、清除非測試 To/CC/BCC、BCC 加開發者。
        /// Testing:自動加主旨[測試]。
        /// </summary>
        private async Task SendAsync(MailMessage message)
        {
            try
            {
                if (SystemConfig == SystemConfig.Develop)
                {
                    if (!message.Subject.StartsWith("[開發]")) { message.Subject = "[開發]" + message.Subject; }
                    message.To.Where(c => !IsTestEmail(c.Address)).ToList().ForEach(c => message.To.Remove(c));
                    message.CC.Where(c => !IsTestEmail(c.Address)).ToList().ForEach(c => message.CC.Remove(c));
                    message.Bcc.Where(c => !IsTestEmail(c.Address)).ToList().ForEach(c => message.Bcc.Remove(c));
                    message.Bcc.Add(new MailAddress(_options.DeveloperEmail));
                }
                else if (SystemConfig == SystemConfig.Testing)
                {
                    if (!message.Subject.StartsWith("[測試]")) { message.Subject = "[測試]" + message.Subject; }
                }

                using var smtpClient = CreateSmtpClient();
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                await SendTextMessageAsync(ex.Message);
            }
        }

        public async Task SendTextMessageAsync(string msg)
        {
            try
            {
                using var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_options.SenderEmail);
                mailMessage.To.Add(new MailAddress(_options.DeveloperEmail));
                mailMessage.Subject = $"{StrText.UserWebName} - 系統訊息";
                if (SystemConfig == SystemConfig.Testing)
                {
                    mailMessage.Subject = $"[測試] {mailMessage.Subject}";
                }
                else if (SystemConfig == SystemConfig.Develop)
                {
                    mailMessage.Subject = $"[開發] {mailMessage.Subject}";
                }
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = msg;

                using var smtpClient = CreateSmtpClient();
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch
            {
                // 系統訊息信本身失敗就吞掉,避免無限遞迴
            }
        }

        // 基本發信通知(單一收件者)
        public async Task SendMailAsync(MailInfo info)
        {
            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_options.SenderEmail);
            foreach (var item in info.Receivers)
                mailMessage.To.Add(new MailAddress(item.Address, item.DisplayName));
            foreach (var item in info.CCs)
                mailMessage.CC.Add(new MailAddress(item.Address, item.DisplayName));
            foreach (var item in info.BCCs)
                mailMessage.Bcc.Add(new MailAddress(item.Address, item.DisplayName));
            mailMessage.Subject = info.Subject;
            mailMessage.Body = info.Body;
            mailMessage.IsBodyHtml = info.IsBodyHtml;

            await SendAsync(mailMessage);
        }
    }
}
