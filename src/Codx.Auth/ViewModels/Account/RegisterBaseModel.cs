using Codx.Auth.Helpers.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels.Account
{
    public class RegisterBaseModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(100)]
        [Display(Name = "Middle Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [CustomEmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

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
