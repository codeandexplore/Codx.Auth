using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientGrantTypeViewModels
    {
    }

    public class ClientGrantTypeDetailsViewModel 
    {
        public int Id { get; set; }
        public string GrantType { get; set; }
    }

    public class ClientGrantTypesDetailsViewModel
    {
        public ClientGrantTypesDetailsViewModel()
        {
            Claims = new List<ClientGrantTypeDetailsViewModel>();
        }

        public int ClientId { get; set; }
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public List<ClientGrantTypeDetailsViewModel> Claims { get; set; }
    }


    public class ClientGrantTypeAddViewModel : BaseGrantTypeClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class ClientGrantTypeEditViewModel : BaseGrantTypeClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class BaseGrantTypeClaimViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Required]
        [StringLength(250)]
        public string GrantType { get; set; }

    }


}
