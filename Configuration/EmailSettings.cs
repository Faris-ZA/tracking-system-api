namespace WebApplication2.Configuration
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } =
            string.Empty;

        public int SmtpPort { get; set; } = 587;

        public string Username { get; set; } =
            string.Empty;

        public string Password { get; set; } =
            string.Empty;

        public string FromEmail { get; set; } =
            string.Empty;

        public string RecipientEmail { get; set; } =
            string.Empty;

        public bool UseSsl { get; set; } = true;
    }
}