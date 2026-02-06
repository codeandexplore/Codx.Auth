using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codx.Auth.Configuration;
using Codx.Auth.Models.Email;
using Codx.Auth.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace Codx.Auth.Infrastructure.Email
{
    /// <summary>
    /// Mailgun implementation of email service
    /// </summary>
    public class MailgunEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<MailgunEmailService> _logger;
        private readonly RestClient _restClient;

        public MailgunEmailService(IOptions<EmailSettings> emailSettings, ILogger<MailgunEmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;

            if (!_emailSettings.Mailgun.IsConfigured)
            {
                _logger.LogWarning("Mailgun is not properly configured. Please check your EmailSettings configuration.");
            }

            var options = new RestClientOptions(_emailSettings.Mailgun.BaseUrl)
            {
                Authenticator = new HttpBasicAuthenticator("api", _emailSettings.Mailgun.ApiKey)
            };
            
            _restClient = new RestClient(options);
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

                if (!_emailSettings.Mailgun.IsConfigured)
                {
                    var error = "Mailgun is not properly configured";
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

                var request = CreateMailgunRequest(message);
                var response = await _restClient.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", 
                        message.To, message.Subject);
                    
                    // Extract message ID from response if available
                    var messageId = ExtractMessageIdFromResponse(response.Content);
                    return EmailResult.SuccessResult(messageId, "Email sent successfully via Mailgun");
                }
                else
                {
                    var errorMessage = $"Failed to send email via Mailgun. Status: {response.StatusCode}, Error: {response.ErrorMessage}, Content: {response.Content}";
                    _logger.LogError(errorMessage);
                    return EmailResult.FailureResult(errorMessage);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Exception occurred while sending email via Mailgun: {ex.Message}";
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

        private RestRequest CreateMailgunRequest(EmailMessage message)
        {
            var request = new RestRequest($"/{_emailSettings.Mailgun.Domain}/messages", Method.Post);

            // From
            var fromAddress = !string.IsNullOrWhiteSpace(message.FromName) 
                ? $"{message.FromName} <{message.From}>" 
                : message.From;
            request.AddParameter("from", fromAddress);

            // To
            var toAddress = !string.IsNullOrWhiteSpace(message.ToName) 
                ? $"{message.ToName} <{message.To}>" 
                : message.To;
            request.AddParameter("to", toAddress);

            // Subject and body
            request.AddParameter("subject", message.Subject);
            
            if (message.IsHtml)
            {
                request.AddParameter("html", message.Body);
            }
            else
            {
                request.AddParameter("text", message.Body);
            }

            // CC recipients
            foreach (var cc in message.Cc)
            {
                request.AddParameter("cc", cc);
            }

            // BCC recipients
            foreach (var bcc in message.Bcc)
            {
                request.AddParameter("bcc", bcc);
            }

            // Custom data
            foreach (var data in message.CustomData)
            {
                request.AddParameter($"v:{data.Key}", data.Value);
            }

            // Attachments
            foreach (var attachment in message.Attachments)
            {
                request.AddFile("attachment", attachment.Content, attachment.FileName, attachment.ContentType);
            }

            return request;
        }

        private string ExtractMessageIdFromResponse(string? responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
                return Guid.NewGuid().ToString();

            try
            {
                // Mailgun returns something like: {"id": "<message-id>", "message": "Queued. Thank you."}
                // This is a simple extraction - you might want to use a JSON parser for production
                var idStart = responseContent.IndexOf("\"id\":");
                if (idStart >= 0)
                {
                    var valueStart = responseContent.IndexOf("\"", idStart + 5);
                    var valueEnd = responseContent.IndexOf("\"", valueStart + 1);
                    if (valueStart >= 0 && valueEnd >= 0)
                    {
                        return responseContent.Substring(valueStart + 1, valueEnd - valueStart - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract message ID from Mailgun response");
            }

            return Guid.NewGuid().ToString();
        }

        public void Dispose()
        {
            _restClient?.Dispose();
        }
    }
}