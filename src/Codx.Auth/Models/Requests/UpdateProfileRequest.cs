using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.Models.Requests
{
    public class UpdateProfileRequest
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string MiddleName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }
    }
}
