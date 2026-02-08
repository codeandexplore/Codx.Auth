using Codx.Auth.Configuration;
using Codx.Auth.Infrastructure.Email;
using Codx.Auth.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Codx.Auth.Extensions
{
    /// <summary>
    /// Extension methods for registering email services
    /// </summary>
    public static class EmailServiceExtensions
    {
        /// <summary>
        /// Registers email services based on configuration
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register configuration
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

            // Register the appropriate email service based on configuration
            services.AddSingleton<IEmailService>(serviceProvider =>
            {
                var emailSettings = serviceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;
                var emailSettingsOptions = serviceProvider.GetRequiredService<IOptions<EmailSettings>>();
                var mailgunLogger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<MailgunEmailService>>();
                var consoleLogger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ConsoleEmailService>>();
                var smtpLogger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<SmtpEmailService>>();

                return emailSettings.Provider.ToLowerInvariant() switch
                {
                    "mailgun" => new MailgunEmailService(emailSettingsOptions, mailgunLogger),
                    "smtp" => new SmtpEmailService(emailSettingsOptions, smtpLogger),
                    "console" => new ConsoleEmailService(consoleLogger, emailSettingsOptions),
                    _ => new ConsoleEmailService(consoleLogger, emailSettingsOptions) // Default to console for unknown providers
                };
            });

            return services;
        }

        /// <summary>
        /// Registers Mailgun email service specifically
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection AddMailgunEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.AddSingleton<IEmailService, MailgunEmailService>();
            return services;
        }

        /// <summary>
        /// Registers console email service specifically (useful for development)
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection AddConsoleEmailService(this IServiceCollection services)
        {
            services.AddSingleton<IEmailService, ConsoleEmailService>();
            return services;
        }
    }
}