namespace YuvaTravel.Infrastructure.Model.Mail
{
    /// <summary>
    /// SMTP 寄信設定。由 appsettings.json 的 "Email" 區塊綁定
    /// (與 R2Options 同模式:Program.cs Bind + AddSingleton)。
    /// </summary>
    public class EmailOptions
    {
        public string Host { get; set; } = "";
        public int PortProduction { get; set; } = 587;
        public int PortTesting { get; set; } = 25;
        public bool EnableSsl { get; set; } = true;
        public string SenderEmail { get; set; } = "";
        public string Password { get; set; } = "";
        public string DeveloperEmail { get; set; } = "";
    }
}
