using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Description("Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [Description("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Description("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
