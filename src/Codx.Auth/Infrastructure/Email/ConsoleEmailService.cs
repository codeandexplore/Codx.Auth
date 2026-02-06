using System;
using System.Threading.Tasks;
using Codx.Auth.Configuration;
using Codx.Auth.Models.Email;
using Codx.Auth.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codx.Auth.Infrastructure.Email
{
    /// <summary>
    /// Console-based email service for development and testing
    /// </summary>
    public class ConsoleEmailService : IEmailService
    {
        private readonly ILogger<ConsoleEmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public ConsoleEmailService(ILogger<ConsoleEmailService> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public Task<EmailResult> SendEmailAsync(EmailMessage message)
        {
            // Set default From if not provided
            if (string.IsNullOrWhiteSpace(message.From))
            {
                message.From = _emailSettings.DefaultFromEmail;
                _logger.LogDebug("Using default From email: {From}", message.From);
            }

            // Set default FromName if not provided
            if (string.IsNullOrWhiteSpace(message.FromName))
            {
                message.FromName = _emailSettings.DefaultFromName;
                _logger.LogDebug("Using default FromName: {FromName}", message.FromName);
            }

            _logger.LogInformation("=== EMAIL WOULD BE SENT ===");
            _logger.LogInformation("From: {From} ({FromName})", message.From, message.FromName);
            _logger.LogInformation("To: {To} ({ToName})", message.To, message.ToName);
            _logger.LogInformation("Subject: {Subject}", message.Subject);
            _logger.LogInformation("Is HTML: {IsHtml}", message.IsHtml);
            
            if (message.Cc.Count > 0)
                _logger.LogInformation("CC: {Cc}", string.Join(", ", message.Cc));
            
            if (message.Bcc.Count > 0)
                _logger.LogInformation("BCC: {Bcc}", string.Join(", ", message.Bcc));
            
            _logger.LogInformation("Body:\n{Body}", message.Body);
            
            if (message.Attachments.Count > 0)
                _logger.LogInformation("Attachments: {AttachmentCount}", message.Attachments.Count);
            
            _logger.LogInformation("=== END EMAIL ===");

            var messageId = $"console-{Guid.NewGuid()}";
            return Task.FromResult(EmailResult.SuccessResult(messageId, "Email logged to console"));
        }

        public Task<EmailResult> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            var message = new EmailMessage
            {
                To = to,
                From = _emailSettings.DefaultFromEmail,
                FromName = _emailSettings.DefaultFromName,
                Subject = subject,
                Body = body,
                IsHtml = isHtml
            };

            return SendEmailAsync(message);
        }
    }
}