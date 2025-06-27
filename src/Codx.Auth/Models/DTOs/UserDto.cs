using Codx.Auth.Data.Entities.AspNet;
using System;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.Models.DTOs
{
    public class UserViewDto
    {
        public UserViewDto()
        {
            
        }

        public UserViewDto(ApplicationUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(256)]
        public string PhoneNumber { get; set; }
    }
}
