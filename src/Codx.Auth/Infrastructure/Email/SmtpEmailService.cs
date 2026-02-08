using System;
using System.Threading.Tasks;
using Codx.Auth.Configuration;
using Codx.Auth.Models.Email;
using Codx.Auth.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Codx.Auth.Infrastructure.Email
{
    /// <summary>
    /// SMTP implementation of email service using MailKit
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;

            if (!_emailSettings.Smtp.IsConfigured)
            {
                _logger.LogWarning("SMTP is not properly configured. Please check your EmailSettings configuration.");
            }
        }

        public async Task<EmailResult> SendEmailAsync(EmailMessage message)
        {
            try
            {
                if (!_emailSettings.EnableEmailSending)
                {
                    _logger.LogInformation("Email sending is disabled. Email would be sent to: {To} with subject: {Subject}",
                        message.To, message.Subject);
                    return EmailResult.SuccessResult("email-disabled", "Email sending is disabled");
                }

                if (!_emailSettings.Smtp.IsConfigured)
                {
                    var error = "SMTP is not properly configured";
                    _logger.LogError(error);
                    return EmailResult.FailureResult(error);
                }

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

                using var mimeMessage = CreateMimeMessage(message);
                using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

                // Determine the secure socket options based on port and settings
                var secureSocketOptions = GetSecureSocketOptions();

                await smtpClient.ConnectAsync(
                    _emailSettings.Smtp.Server,
                    _emailSettings.Smtp.Port,
                    secureSocketOptions);

                if (_emailSettings.Smtp.RequiresAuthentication)
                {
                    await smtpClient.AuthenticateAsync(
                        _emailSettings.Smtp.User,
                        _emailSettings.Smtp.Password);
                }

                var response = await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);

                var messageId = mimeMessage.MessageId ?? $"smtp-{Guid.NewGuid()}";
                _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}",
                    message.To, message.Subject);

                return EmailResult.SuccessResult(messageId, "Email sent successfully via SMTP");
            }
            catch (Exception ex)
            {
                var errorMessage = $"Exception occurred while sending email via SMTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return EmailResult.FailureResult(errorMessage);
            }
        }

        public async Task<EmailResult> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
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

            return await SendEmailAsync(message);
        }

        private SecureSocketOptions GetSecureSocketOptions()
        {
            if (!_emailSettings.Smtp.UseSsl)
            {
                return SecureSocketOptions.None;
            }

            // Port 465 typically uses implicit SSL (SslOnConnect)
            // Port 587 typically uses STARTTLS
            return _emailSettings.Smtp.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;
        }

        private MimeMessage CreateMimeMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();

            // From
            mimeMessage.From.Add(new MailboxAddress(
                message.FromName ?? message.From,
                message.From));

            // To
            mimeMessage.To.Add(new MailboxAddress(
                message.ToName ?? message.To,
                message.To));

            // Subject
            mimeMessage.Subject = message.Subject;

            // CC recipients
            foreach (var cc in message.Cc)
            {
                mimeMessage.Cc.Add(MailboxAddress.Parse(cc));
            }

            // BCC recipients
            foreach (var bcc in message.Bcc)
            {
                mimeMessage.Bcc.Add(MailboxAddress.Parse(bcc));
            }

            // Body and attachments
            var builder = new BodyBuilder();

            if (message.IsHtml)
            {
                builder.HtmlBody = message.Body;
            }
            else
            {
                builder.TextBody = message.Body;
            }

            // Add attachments
            foreach (var attachment in message.Attachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
            }

            mimeMessage.Body = builder.ToMessageBody();

            return mimeMessage;
        }
    }
}
