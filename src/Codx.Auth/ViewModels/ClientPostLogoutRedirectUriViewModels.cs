using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientPostLogoutRedirectUriDetailsViewModel : BasePostLogoutRedirectUriClaimViewModel
    {
        public int Id { get; set; }
    }

    public class ClientPostLogoutRedirectUriAddViewModel : BasePostLogoutRedirectUriClaimViewModel
    {
    }

    public class ClientPostLogoutRedirectUriEditViewModel : BasePostLogoutRedirectUriClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BasePostLogoutRedirectUriClaimViewModel
    {

        public int ClientId { get; set; }

        [Required]
        [StringLength(2000)]
        public string PostLogoutRedirectUri { get; set; }

    }


}
