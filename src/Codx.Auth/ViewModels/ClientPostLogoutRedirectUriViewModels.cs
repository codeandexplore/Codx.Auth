using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientPostLogoutRedirectUriViewModels
    {
    }

    public class ClientPostLogoutRedirectUriDetailsViewModel 
    {
        public int Id { get; set; }
        public string PostLogoutRedirectUri { get; set; }
    }

    public class ClientPostLogoutRedirectUrisDetailsViewModel
    {
        public ClientPostLogoutRedirectUrisDetailsViewModel()
        {
            PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUriDetailsViewModel>();
        }

        public int ClientId { get; set; }
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public List<ClientPostLogoutRedirectUriDetailsViewModel> PostLogoutRedirectUris { get; set; }
    }


    public class ClientPostLogoutRedirectUriAddViewModel : BasePostLogoutRedirectUriClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class ClientPostLogoutRedirectUriEditViewModel : BasePostLogoutRedirectUriClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class BasePostLogoutRedirectUriClaimViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Required]
        [StringLength(2000)]
        public string PostLogoutRedirectUri { get; set; }

    }


}
