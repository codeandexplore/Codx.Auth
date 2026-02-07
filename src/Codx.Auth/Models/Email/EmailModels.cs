using System.Collections.Generic;

namespace Codx.Auth.Models.Email
{
    /// <summary>
    /// Represents an email message
    /// </summary>
    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string? ToName { get; set; }
        public string From { get; set; } = string.Empty;
        public string? FromName { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = false;
        public List<string> Cc { get; set; } = new();
        public List<string> Bcc { get; set; } = new();
        public Dictionary<string, string> CustomData { get; set; } = new();
        public List<EmailAttachment> Attachments { get; set; } = new();
    }

    /// <summary>
    /// Represents an email attachment
    /// </summary>
    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = System.Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
    }

    /// <summary>
    /// Result of an email sending operation
    /// </summary>
    public class EmailResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? MessageId { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new();

        public static EmailResult SuccessResult(string messageId, string message = "Email sent successfully")
        {
            return new EmailResult
            {
                Success = true,
                Message = message,
                MessageId = messageId
            };
        }

        public static EmailResult FailureResult(string message)
        {
            return new EmailResult
            {
                Success = false,
                Message = message
            };
        }
    }
}