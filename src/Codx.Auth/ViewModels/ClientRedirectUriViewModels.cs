using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientRedirectUriDetailsViewModel : BaseRedirectUriClaimViewModel
    {
        public int Id { get; set; }
    }

    public class ClientRedirectUriAddViewModel : BaseRedirectUriClaimViewModel
    {
    }

    public class ClientRedirectUriEditViewModel : BaseRedirectUriClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseRedirectUriClaimViewModel
    {
        public int ClientId { get; set; }

        [Required]
        [StringLength(2000)]
        public string RedirectUri { get; set; }

    }


}
