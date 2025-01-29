using System;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{
    public class TenantManagerDetailsViewModel : BaseTenantManagerViewModel
    {
    }

    public class TenantManagerAddViewModel : BaseTenantManagerViewModel
    {      
    }

    public class TenantManagerEditViewModel : BaseTenantManagerViewModel
    {
    }

    public class BaseTenantManagerViewModel
    {
        [Required]
        public Guid TenantId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [StringLength(100)]
        public string UserEmail { get; set; }
        [StringLength(100)]
        public string UserName { get; set; }
    }

}
