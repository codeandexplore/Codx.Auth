using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Helpers.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.Models.DTOs
{
    public class TenantViewDto
    {
        public TenantViewDto()
        {
            
        }

        public TenantViewDto(Tenant tenant)
        {
            Id = tenant.Id;
            Name = tenant.Name;
            Email = tenant.Email;
            Phone = tenant.Phone;
            Address = tenant.Address;
            Logo = tenant.Logo;
            Theme = tenant.Theme;
            Description = tenant.Description;
        }

        public Guid Id { get; set; }     
        public string Name { get; set; }      
        public string Email { get; set; }    
        public string Phone { get; set; }       
        public string Address { get; set; }       
        public string Logo { get; set; }    
        public string Theme { get; set; }       
        public string Description { get; set; }
    }

    public class TenantAddDto : TenantBaseDto { }
    public class TenantEditDto : TenantBaseDto
    {
        public Guid Id { get; set; }
    }

    public class TenantBaseDto
    {
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
