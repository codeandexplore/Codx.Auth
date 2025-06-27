using System.ComponentModel.DataAnnotations;
using System;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Helpers.CustomAttributes;

namespace Codx.Auth.Models.DTOs
{
    public class CompanyViewDto
    {
        public CompanyViewDto()
        {
            
        }

        public CompanyViewDto(Company company)
        {
            Id = company.Id;
            TenantId = company.TenantId;
            TenantName = company.Tenant.Name;
            Name = company.Name;
            Email = company.Email;
            Phone = company.Phone;
            Address = company.Address;
            Logo = company.Logo;
            Theme = company.Theme;
            Description = company.Description;
        }
        public Guid Id { get; set; }      
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Logo { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
    }

    public class CompanyAddDto : CompanyBaseDto { }
    public class CompanyEditDto: CompanyBaseDto
    {
        public Guid Id { get; set; }
        
    }

    public class CompanyBaseDto
    {
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
