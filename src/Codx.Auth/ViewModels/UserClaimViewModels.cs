using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{

    public class UserClaimDetailsViewModel : BaseUserClaimViewModel
    {
        public int Id { get; set; }
    }

    public class UserClaimAddViewModel : BaseUserClaimViewModel
    {
    }

    public class UserClaimEditViewModel : BaseUserClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseUserClaimViewModel
    {      

        public Guid UserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string ClaimType { get; set; }

        [Required]
        [StringLength(1000)]
        public string ClaimValue { get; set; }
    }
}
