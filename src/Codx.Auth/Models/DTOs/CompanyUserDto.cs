using Codx.Auth.Helpers.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.Models.DTOs
{
    public class CompanyUserAddDto
    {
        [Required(ErrorMessage = "Email address is required")]
        [CustomEmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public string EmailAddress { get; set; }
    }
}
