using System;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class UserCompanyDetailsViewModel : BaseUserCompanyViewModel
    {
    }

    public class UserCompanyAddViewModel : BaseUserCompanyViewModel
    {      
    }

    public class UserCompanyEditViewModel : BaseUserCompanyViewModel
    {
    }

    public class BaseUserCompanyViewModel
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid CompanyId { get; set; }

        [StringLength(100)]
        public string CompanyName { get; set; }
        [StringLength(100)]
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
    }

}
