using Codx.Auth.Helpers.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(100)]
        [Display(Name = "Middle Name")]
        public string LastName { get; set; }

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

        public string ReturnUrl { get; set; }
    }
}
