using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientRedirectUriViewModels
    {
    }

    public class ClientRedirectUriDetailsViewModel 
    {
        public int Id { get; set; }
        public string RedirectUri { get; set; }
    }

    public class ClientRedirectUrisDetailsViewModel
    {
        public ClientRedirectUrisDetailsViewModel()
        {
            RedirectUris = new List<ClientRedirectUriDetailsViewModel>();
        }

        public int ClientId { get; set; }
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public List<ClientRedirectUriDetailsViewModel> RedirectUris { get; set; }
    }


    public class ClientRedirectUriAddViewModel : BaseRedirectUriClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class ClientRedirectUriEditViewModel : BaseRedirectUriClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class BaseRedirectUriClaimViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Required]
        [StringLength(2000)]
        public string RedirectUri { get; set; }

    }


}
