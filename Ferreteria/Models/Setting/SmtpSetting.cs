namespace Ferreteria.Models.Setting
{
    public class SmtpSetting
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string User { get; set; } = null;
        public string Password { get; set; } = null;
    }
}
