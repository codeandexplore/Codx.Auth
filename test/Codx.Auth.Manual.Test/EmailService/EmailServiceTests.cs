using Codx.Auth.Manual.Test.Infrastructure;
using Codx.Auth.Models.Email;
using Codx.Auth.Services.Interfaces;

namespace Codx.Auth.Manual.Test.EmailService
{
    public class EmailServiceTests : IClassFixture<EmailServiceTestFixture>
    {
        private readonly EmailServiceTestFixture _fixture;

        private readonly string _mailGunToEmail = "MAILGUN_TO_EMAIL";

        public EmailServiceTests(EmailServiceTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SendEmailMailGun_ShouldReturnSuccess()
        {
            // Arrange
            var emailService = _fixture.CreateMailgunEmailService();
            var emailMessage = new EmailMessage
            {
                From = "test@example.com",
                To = _mailGunToEmail,
                Subject = "Test",
                Body = "This is a test email.",
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
        public async Task SendEmailConsole_ShouldReturnSuccess()
        {
            // Arrange
            var emailService = _fixture.CreateConsoleEmailService();
            var emailMessage = new EmailMessage
            {
                From = "test@console.local",
                To = "console@example.com",
                Subject = "Console Test",
                Body = "This is a test email to the console.",
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
        public async Task SendEmailMailGun_WithSimpleParameters_ShouldReturnSuccess()
        {
            // Arrange
            var emailService = _fixture.CreateMailgunEmailService();

            // Act
            var result = await emailService.SendEmailAsync(
                to: _mailGunToEmail,
                subject: "Simple Test",
                body: "This is a simple test email.",
                isHtml: false
            );

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success, $"Email sending failed: {result.Message}");
            Assert.NotNull(result.MessageId);
            Assert.NotEmpty(result.MessageId);
        }

        [Fact]
        public async Task SendEmailConsole_WithSimpleParameters_ShouldReturnSuccess()
        {
            // Arrange
            var emailService = _fixture.CreateConsoleEmailService();

            // Act
            var result = await emailService.SendEmailAsync(
                to: "console@example.com",
                subject: "Simple Console Test",
                body: "This is a simple test email to the console.",
                isHtml: false
            );

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success, $"Email sending failed: {result.Message}");
            Assert.NotNull(result.MessageId);
            Assert.NotEmpty(result.MessageId);
        }

        [Fact]
        public async Task SendEmailMailGun_WithHtmlContent_ShouldReturnSuccess()
        {
            // Arrange
            var emailService = _fixture.CreateMailgunEmailService();
            var emailMessage = new EmailMessage
            {
                From = "test@example.com",
                To = _mailGunToEmail,
                Subject = "HTML Test Updated",
                Body = "<h1>Updated Test Email</h1><p>This is an <strong>updated test email</strong> with HTML content.</p>",
                IsHtml = true
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
        public async Task SendEmailConsole_WithHtmlContent_ShouldReturnSuccess()
        {
            // Arrange
            var emailService = _fixture.CreateConsoleEmailService();
            var emailMessage = new EmailMessage
            {
                From = "test@console.local",
                To = "console@example.com",
                Subject = "HTML Console Test",
                Body = "<h1>Console Test Email</h1><p>This is a <strong>test email</strong> with HTML content for console output.</p>",
                IsHtml = true
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
    }
}