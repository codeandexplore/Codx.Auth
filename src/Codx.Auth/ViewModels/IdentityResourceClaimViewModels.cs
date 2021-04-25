using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{

    public class IdentityResourceClaimDetailsViewModel : BaseIdentityResourceClaimViewModel
    {
        public int Id { get; set; }
    }

    public class IdentityResourceClaimAddViewModel : BaseIdentityResourceClaimViewModel
    {
    }

    public class IdentityResourceClaimEditViewModel : BaseIdentityResourceClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseIdentityResourceClaimViewModel
    {
        public int IdentityResourceId { get; set; }

        [Required]
        [StringLength(200)]
        public string Type { get; set; }
    }
}
