using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Codx.Auth.Extensions;
using Codx.Auth.Models.Email;
using Codx.Auth.Services.Interfaces;

namespace Codx.Auth.Manual.Test.EmailService
{
    /// <summary>
    /// Integration tests that test email services through the dependency injection container
    /// </summary>
    public class EmailServiceIntegrationTests
    {
        [Fact]
        public async Task EmailService_WithMailgunConfiguration_ShouldUseMailgunService()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider("appsettings.Test.json");
            var emailService = serviceProvider.GetRequiredService<IEmailService>();
            
            var emailMessage = new EmailMessage 
            { 
                From = "test@example.com",
                To = "integration-test@example.com", 
                Subject = "Integration Test - Mailgun", 
                Body = "This is an integration test email using Mailgun configuration.",
                IsHtml = false
            };

            // Act
            var result = await emailService.SendEmailAsync(emailMessage);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success, $"Email sending failed: {result.Message}");
            Assert.NotNull(result.MessageId);
            Assert.NotEmpty(result.MessageId);
        }

        [Fact]
        public async Task EmailService_WithConsoleConfiguration_ShouldUseConsoleService()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider("appsettings.Console.json");
            var emailService = serviceProvider.GetRequiredService<IEmailService>();
            
            var emailMessage = new EmailMessage 
            { 
                From = "test@console.local",
                To = "integration-test@console.local", 
                Subject = "Integration Test - Console", 
                Body = "This is an integration test email using Console configuration.",
                IsHtml = false
            };

            // Act
            var result = await emailService.SendEmailAsync(emailMessage);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success, $"Email sending failed: {result.Message}");
            Assert.NotNull(result.MessageId);
            Assert.NotEmpty(result.MessageId);
            Assert.Contains("console", result.MessageId);
        }

        [Fact]
        public async Task EmailService_WithInvalidMailgunConfiguration_ShouldHandleGracefully()
        {
            // Arrange
            var serviceProvider = CreateServiceProviderWithInvalidMailgunConfig();
            var emailService = serviceProvider.GetRequiredService<IEmailService>();
            
            var emailMessage = new EmailMessage 
            { 
                From = "test@example.com",
                To = "test@example.com", 
                Subject = "Invalid Config Test", 
                Body = "This should fail gracefully with invalid Mailgun configuration.",
                IsHtml = false
            };

            // Act
            var result = await emailService.SendEmailAsync(emailMessage);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("not properly configured", result.Message);
        }

        private static IServiceProvider CreateServiceProvider(string configFileName)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFileName, optional: false, reloadOnChange: true)
                .Build();

            // Setup service collection
            var services = new ServiceCollection();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddConfiguration(configuration.GetSection("Logging"));
            });

            // Add email services
            services.AddEmailServices(configuration);

            return services.BuildServiceProvider();
        }

        private static IServiceProvider CreateServiceProviderWithInvalidMailgunConfig()
        {
            // Build configuration with invalid Mailgun settings
            var configData = new Dictionary<string, string>
            {
                {"EmailSettings:Provider", "Mailgun"},
                {"EmailSettings:DefaultFromEmail", "test@example.com"},
                {"EmailSettings:DefaultFromName", "Test Sender"},
                {"EmailSettings:EnableEmailSending", "true"},
                {"EmailSettings:Mailgun:ApiKey", ""}, // Invalid - empty
                {"EmailSettings:Mailgun:Domain", ""}, // Invalid - empty
                {"EmailSettings:Mailgun:BaseUrl", "https://api.mailgun.net/v3"},
                {"Logging:LogLevel:Default", "Information"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();

            // Setup service collection
            var services = new ServiceCollection();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddConfiguration(configuration.GetSection("Logging"));
            });

            // Add email services
            services.AddEmailServices(configuration);

            return services.BuildServiceProvider();
        }
    }
}