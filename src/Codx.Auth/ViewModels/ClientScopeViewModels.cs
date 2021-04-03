using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientScopeViewModels
    {
    }

    public class ClientScopeDetailsViewModel 
    {
        public int Id { get; set; }
        public string Scope { get; set; }
    }

    public class ClientScopesDetailsViewModel
    {
        public ClientScopesDetailsViewModel()
        {
            Scopes = new List<ClientScopeDetailsViewModel>();
        }

        public int ClientId { get; set; }
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public List<ClientScopeDetailsViewModel> Scopes { get; set; }
    }


    public class ClientScopeAddViewModel : BaseScopeClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class ClientScopeEditViewModel : BaseScopeClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class BaseScopeClaimViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Required]
        [StringLength(250)]
        public string Scope { get; set; }

    }


}
