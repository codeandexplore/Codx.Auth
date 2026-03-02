using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class UserDetailsViewModel : BaseUserEditViewModel
    { 
        public Guid Id { get; set; }
        public string TenantName { get; set; }
        public string CompanyName { get; set; }
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
        public Guid? DefaultCompanyId { get; set; }
        public List<SelectListItem> CompanySelectOptions { get; set; }
    }

    public class UserResetPasswordViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        [Required]
        [StringLength(256)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(256)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
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
