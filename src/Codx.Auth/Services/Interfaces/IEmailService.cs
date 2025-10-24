using System.Threading.Tasks;
using Codx.Auth.Models.Email;

namespace Codx.Auth.Services.Interfaces
{
    /// <summary>
    /// Service for sending emails
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email message
        /// </summary>
        /// <param name="message">The email message to send</param>
        /// <returns>Result indicating success or failure</returns>
        Task<EmailResult> SendEmailAsync(EmailMessage message);

        /// <summary>
        /// Sends a simple email with subject and body
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="isHtml">Whether the body is HTML formatted</param>
        /// <returns>Result indicating success or failure</returns>
        Task<EmailResult> SendEmailAsync(string to, string subject, string body, bool isHtml = false);      
    }
}