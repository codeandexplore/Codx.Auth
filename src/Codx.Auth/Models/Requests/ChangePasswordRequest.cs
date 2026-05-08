using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.Models.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required, MinLength(8)]
        public string NewPassword { get; set; }
    }
}
