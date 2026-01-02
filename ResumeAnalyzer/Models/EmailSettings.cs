namespace ResumeAnalyzer.Models
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = null!;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string From { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }

}
