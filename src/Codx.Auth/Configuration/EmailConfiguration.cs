namespace Codx.Auth.Configuration
{
    /// <summary>
    /// Configuration settings for email services
    /// </summary>
    public class EmailSettings
    {
        public const string SectionName = "EmailSettings";
        
        /// <summary>
        /// Email service provider (e.g., "Mailgun", "Smtp", "Console")
        /// </summary>
        public string Provider { get; set; } = "Mailgun";
        
        /// <summary>
        /// Default sender email address
        /// </summary>
        public string DefaultFromEmail { get; set; } = string.Empty;
        
        /// <summary>
        /// Default sender name
        /// </summary>
        public string DefaultFromName { get; set; } = string.Empty;
        
        /// <summary>
        /// Mailgun specific settings
        /// </summary>
        public MailgunSettings Mailgun { get; set; } = new();
        
        /// <summary>
        /// SMTP specific settings
        /// </summary>
        public SmtpSettings Smtp { get; set; } = new();
        
        /// <summary>
        /// Whether to enable email sending (useful for development/testing)
        /// </summary>
        public bool EnableEmailSending { get; set; } = true;
    }

    /// <summary>
    /// Mailgun specific configuration
    /// </summary>
    public class MailgunSettings
    {
        /// <summary>
        /// Mailgun API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
        
        /// <summary>
        /// Mailgun domain
        /// </summary>
        public string Domain { get; set; } = string.Empty;
        
        /// <summary>
        /// Mailgun API base URL (default: https://api.mailgun.net/v3)
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.mailgun.net/v3";
        
        /// <summary>
        /// Whether this configuration is properly set up
        /// </summary>
        public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(Domain);
    }

    /// <summary>
    /// SMTP specific configuration
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// SMTP server hostname
        /// </summary>
        public string Server { get; set; } = string.Empty;
        
        /// <summary>
        /// SMTP server port
        /// </summary>
        public int Port { get; set; } = 587;
        
        /// <summary>
        /// SMTP username for authentication
        /// </summary>
        public string User { get; set; } = string.Empty;
        
        /// <summary>
        /// SMTP password for authentication
        /// </summary>
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether SMTP requires authentication
        /// </summary>
        public bool RequiresAuthentication { get; set; } = true;
        
        /// <summary>
        /// Whether to use SSL/TLS
        /// </summary>
        public bool UseSsl { get; set; } = true;
        
        /// <summary>
        /// Whether this configuration is properly set up
        /// </summary>
        public bool IsConfigured => !string.IsNullOrWhiteSpace(Server) && Port > 0;
    }
}