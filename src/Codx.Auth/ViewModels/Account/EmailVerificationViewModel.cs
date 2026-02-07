namespace Codx.Auth.ViewModels.Account
{
    public class EmailVerificationViewModel
    {
        public string Email { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class EmailConfirmedViewModel
    {
        public string ReturnUrl { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}