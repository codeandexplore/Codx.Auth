using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class UserDetailsViewModel : BaseUserEditViewModel
    { 
        public Guid Id { get; set; }
    }

    public class UserAddViewModel : BaseUserEditViewModel
    {
        [Required]
        [StringLength(256)]        
        public string Password { get; set; }

        [Required]
        [StringLength(256)]
        public string ConfirmPassword { get; set; }
    }

    public class UserEditViewModel : BaseUserEditViewModel
    {

        public Guid Id { get; set; }

    }

    public class BaseUserEditViewModel
    {

        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(256)]
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }

    }
}
