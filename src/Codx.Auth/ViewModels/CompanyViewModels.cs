using Codx.Auth.Helpers.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class CompanyDetailsViewModel : BaseCompanyViewModel
    {
        public Guid  Id { get; set; }
    }

    public class CompanyAddViewModel : BaseCompanyViewModel
    { 
    }

    public class CompanyEditViewModel : BaseCompanyViewModel
    {
        public Guid Id { get; set; }
    }
    public class BaseCompanyViewModel
    {
        [Required]
        public Guid TenantId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        [CustomEmailAddress]
        public string Email { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        [StringLength(200)]
        public string Logo { get; set; }
        [StringLength(50)]
        public string Theme { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
    }

}
