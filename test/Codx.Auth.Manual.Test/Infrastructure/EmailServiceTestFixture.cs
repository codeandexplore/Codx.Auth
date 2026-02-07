using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Codx.Auth.Extensions;
using Codx.Auth.Services.Interfaces;
using Codx.Auth.Configuration;
using Codx.Auth.Infrastructure.Email;

namespace Codx.Auth.Manual.Test.Infrastructure
{
    /// <summary>
    /// Test fixture for setting up dependency injection and services for email service tests
    /// </summary>
    public class EmailServiceTestFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public EmailServiceTestFixture()
        {
            SetupServices();
        }

        private void SetupServices()
        {
            // Build configuration
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);

            Configuration = configurationBuilder.Build();

            // Setup service collection
            var services = new ServiceCollection();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddConfiguration(Configuration.GetSection("Logging"));
            });

            // Add email services
            services.AddEmailServices(Configuration);

            // Build service provider
            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Gets a service of type T from the service provider
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service instance</returns>
        public T GetService<T>() where T : class
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Creates a Mailgun email service for testing
        /// </summary>
        /// <returns>Mailgun email service instance</returns>
        public IEmailService CreateMailgunEmailService()
        {
            var emailSettings = Configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>();
            var logger = GetService<ILogger<MailgunEmailService>>();
            var options = Microsoft.Extensions.Options.Options.Create(emailSettings!);
            
            return new MailgunEmailService(options, logger);
        }

        /// <summary>
        /// Creates a Console email service for testing
        /// </summary>
        /// <returns>Console email service instance</returns>
        public IEmailService CreateConsoleEmailService()
        {
            var emailSettings = Configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>();
            var logger = GetService<ILogger<ConsoleEmailService>>();
            var options = Microsoft.Extensions.Options.Options.Create(emailSettings!);
            
            return new ConsoleEmailService(logger, options);
        }

        public void Dispose()
        {
            if (ServiceProvider is ServiceProvider sp)
            {
                sp.Dispose();
            }
        }
    }
}