using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels.Account
{
    /// <summary>
    /// View model for Two-Factor Authentication verification
    /// </summary>
    public class TwoFactorViewModel
    {
        [Required(ErrorMessage = "Verification code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be 6 digits")]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;

        public bool RememberLogin { get; set; }

        public string Email { get; set; } = string.Empty;

        public bool CanResendCode { get; set; } = true;

        public string Message { get; set; } = string.Empty;

        public bool IsError { get; set; } = false;
    }

    /// <summary>
    /// Result of sending 2FA code
    /// </summary>
    public class SendTwoFactorCodeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // For testing purposes
    }
}