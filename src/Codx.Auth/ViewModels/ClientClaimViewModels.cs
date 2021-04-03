using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientClaimViewModels
    {
    }

    public class ClientClaimDetailsViewModel 
    {
        public int Id { get; set; }

        public int ClaimId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    public class ClientClaimsDetailsViewModel
    {
        public ClientClaimsDetailsViewModel()
        {
            Claims = new List<ClientClaimDetailsViewModel>();
        }

        public int ClientId { get; set; }
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public List<ClientClaimDetailsViewModel> Claims { get; set; }
    }


    public class ClientClaimAddViewModel : BaseClientClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class ClientClaimEditViewModel : BaseClientClaimViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class BaseClientClaimViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Required]
        [StringLength(250)]
        public string ClaimType { get; set; }

        [Required]
        [StringLength(250)]
        public string ClaimValue { get; set; }
    }


}
