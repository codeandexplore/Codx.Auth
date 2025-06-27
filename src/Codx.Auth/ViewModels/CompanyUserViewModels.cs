using Codx.Auth.Helpers.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class CompanyUserDetailsViewModel : BaseCompanyUserViewModel
    {
    }

    public class CompanyUserAddViewModel : BaseCompanyUserViewModel
    {      
    }

    public class CompanyUserEditViewModel : BaseCompanyUserViewModel
    {
    }

    public class BaseCompanyUserViewModel
    {
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [CustomEmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100)]
        public string UserEmail { get; set; }
        [StringLength(100)]
        public string UserName { get; set; }
    }

}
