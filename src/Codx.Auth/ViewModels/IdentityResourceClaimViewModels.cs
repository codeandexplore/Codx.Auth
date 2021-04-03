using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class IdentityResourceClaimViewModels
    {
    }

    public class IdentityResourceClaimDetailsViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class IdentityResourceClaimAddViewModel : BaseIdentityResourceClaimViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class IdentityResourceClaimEditViewModel : BaseIdentityResourceClaimViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class BaseIdentityResourceClaimViewModel
    {
        public int Id { get; set; }
        public int IdentityResourceId { get; set; }

        [Required]
        [StringLength(200)]
        public string Type { get; set; }
    }
}
