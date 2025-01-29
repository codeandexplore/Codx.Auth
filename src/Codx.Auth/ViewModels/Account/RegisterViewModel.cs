using Codx.Auth.Helpers.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [CustomEmailAddress]
        [Display(Name = "Email Address")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
