using System.ComponentModel.DataAnnotations;
using System;
using Codx.Auth.Data.Entities.Enterprise;

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
        [Required]
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
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
