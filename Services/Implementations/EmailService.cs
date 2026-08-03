using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApplication2.Configuration;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> options,
            ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendOfflinePeopleReportAsync(
            string reportFilePath,
            int offlinePeopleCount,
            CancellationToken cancellationToken = default)
        {
            ValidateSettings();

            if (!File.Exists(reportFilePath))
            {
                throw new FileNotFoundException(
                    "The report file was not found.",
                    reportFilePath);
            }

            try
            {
                var message = new MimeMessage();

                message.From.Add(
                    MailboxAddress.Parse(
                        _settings.FromEmail));

                message.To.Add(
                    MailboxAddress.Parse(
                        _settings.RecipientEmail));

                message.Subject =
                    "Offline People Report";

                var bodyBuilder = new BodyBuilder
                {
                    TextBody =
                        "The scheduled offline people report " +
                        "was generated successfully." +
                        Environment.NewLine +
                        $"Offline people count: {offlinePeopleCount}." +
                        Environment.NewLine +
                        "The CSV report is attached."
                };

                await bodyBuilder.Attachments.AddAsync(
                    reportFilePath,
                    cancellationToken);

                message.Body = bodyBuilder.ToMessageBody();

                using var smtpClient = new SmtpClient();

                var socketOptions =
                    _settings.UseSsl
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.None;

                await smtpClient.ConnectAsync(
                    _settings.SmtpHost,
                    _settings.SmtpPort,
                    socketOptions,
                    cancellationToken);

                await smtpClient.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password,
                    cancellationToken);

                await smtpClient.SendAsync(
                    message,
                    cancellationToken);

                await smtpClient.DisconnectAsync(
                    true,
                    cancellationToken);

                _logger.LogInformation(
                    "Offline people report email sent successfully to {RecipientEmail}.",
                    _settings.RecipientEmail);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to send the offline people report email.");

                throw;
            }
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(
                    _settings.SmtpHost))
            {
                throw new InvalidOperationException(
                    "SMTP host is not configured.");
            }

            if (_settings.SmtpPort <= 0)
            {
                throw new InvalidOperationException(
                    "SMTP port is invalid.");
            }

            if (string.IsNullOrWhiteSpace(
                    _settings.Username))
            {
                throw new InvalidOperationException(
                    "SMTP username is not configured.");
            }

            if (string.IsNullOrWhiteSpace(
                    _settings.Password))
            {
                throw new InvalidOperationException(
                    "SMTP password is not configured.");
            }

            if (string.IsNullOrWhiteSpace(
                    _settings.FromEmail))
            {
                throw new InvalidOperationException(
                    "Sender email is not configured.");
            }

            if (string.IsNullOrWhiteSpace(
                    _settings.RecipientEmail))
            {
                throw new InvalidOperationException(
                    "Recipient email is not configured.");
            }
        }
    }
}