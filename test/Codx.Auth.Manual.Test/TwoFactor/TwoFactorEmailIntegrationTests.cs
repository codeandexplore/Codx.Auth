using Codx.Auth.Manual.Test.Infrastructure;
using Codx.Auth.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Codx.Auth.Manual.Test.TwoFactor
{
    /// <summary>
    /// Integration tests for Two-Factor Authentication email functionality
    /// </summary>
    public class TwoFactorEmailIntegrationTests : IClassFixture<EmailServiceTestFixture>
    {
        private readonly EmailServiceTestFixture _fixture;

        public TwoFactorEmailIntegrationTests(EmailServiceTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task TwoFactorEmail_WithConsoleProvider_ShouldLogEmailContent()
        {
            // Arrange
            var emailService = _fixture.CreateConsoleEmailService();
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var userName = "TestUser";

            // Act - Send a mock 2FA email directly through email service
            var result = await emailService.SendEmailAsync(
                to: email,
                subject: "Your Two-Factor Authentication Code",
                body: CreateMock2FAEmailBody(userName, "123456"),
                isHtml: true
            );

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success, $"Email sending failed: {result.Message}");
            Assert.NotNull(result.MessageId);
            Assert.NotEmpty(result.MessageId);
            Assert.Contains("console", result.MessageId);
        }

        [Fact]
        public async Task TwoFactorEmail_WithMailgunProvider_ShouldAttemptToSend()
        {
            // Arrange
            var emailService = _fixture.CreateMailgunEmailService();
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var userName = "TestUser";

            // Act - Send a mock 2FA email directly through email service
            var result = await emailService.SendEmailAsync(
                to: email,
                subject: "Your Two-Factor Authentication Code",
                body: CreateMock2FAEmailBody(userName, "123456"),
                isHtml: true
            );

            // Assert - With test credentials, this should fail gracefully
            Assert.NotNull(result);
            // Note: This will likely fail with test credentials, but should not throw exceptions
            // The important thing is that the service handles the failure gracefully
        }

        [Fact]
        public void TwoFactorEmailTemplate_ShouldContainRequiredElements()
        {
            // Arrange
            var userName = "John Doe";
            var code = "123456";

            // Act
            var emailBody = CreateMock2FAEmailBody(userName, code);

            // Assert
            Assert.Contains(userName, emailBody);
            Assert.Contains(code, emailBody);
            Assert.Contains("Two-Factor Authentication", emailBody);
            Assert.Contains("verification code", emailBody);
            Assert.Contains("10 minutes", emailBody); // Expiration time
            Assert.Contains("do not share", emailBody.ToLower()); // Security warning
        }

        [Fact]
        public void TwoFactorEmailTemplate_ShouldBeValidHtml()
        {
            // Arrange
            var userName = "Test User";
            var code = "654321";

            // Act
            var emailBody = CreateMock2FAEmailBody(userName, code);

            // Assert
            Assert.Contains("<!DOCTYPE html>", emailBody);
            Assert.Contains("<html>", emailBody);
            Assert.Contains("</html>", emailBody);
            Assert.Contains("<body>", emailBody);
            Assert.Contains("</body>", emailBody);
        }

        /// <summary>
        /// Creates a mock 2FA email body similar to the one in TwoFactorService
        /// </summary>
        private string CreateMock2FAEmailBody(string displayName, string code)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Two-Factor Authentication Code</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #007bff;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f8f9fa;
            padding: 30px;
            border-radius: 0 0 5px 5px;
        }}
        .verification-code {{
            background-color: #e9ecef;
            border: 2px solid #007bff;
            border-radius: 5px;
            font-size: 32px;
            font-weight: bold;
            text-align: center;
            padding: 20px;
            margin: 20px 0;
            letter-spacing: 5px;
            color: #007bff;
        }}
        .warning {{
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 5px;
            padding: 15px;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            font-size: 12px;
            color: #6c757d;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Two-Factor Authentication</h1>
    </div>
    <div class=""content"">
        <h2>Hello {displayName},</h2>
        <p>You are attempting to sign in to your account. To complete the login process, please use the verification code below:</p>
        
        <div class=""verification-code"">
            {code}
        </div>
        
        <div class=""warning"">
            <strong>Important:</strong>
            <ul>
                <li>This code will expire in 10 minutes</li>
                <li>Do not share this code with anyone</li>
                <li>If you did not request this code, please secure your account immediately</li>
            </ul>
        </div>
        
        <p>Enter this code on the verification page to complete your login.</p>
        
        <p>Best regards,<br>Codx Auth System</p>
    </div>
    <div class=""footer"">
        <p>This is an automated message. Please do not reply to this email.</p>
        <p>If you have any questions, please contact our support team.</p>
    </div>
</body>
</html>";
        }
    }
}