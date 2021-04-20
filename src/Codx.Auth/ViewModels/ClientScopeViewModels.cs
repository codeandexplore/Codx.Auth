using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{

    public class ClientScopeDetailsViewModel : BaseScopeClaimViewModel
    {
        public int Id { get; set; }
    }


    public class ClientScopeAddViewModel : BaseScopeClaimViewModel
    {
    }

    public class ClientScopeEditViewModel : BaseScopeClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseScopeClaimViewModel
    {
  
        public int ClientId { get; set; }

        [Required]
        [StringLength(250)]
        public string Scope { get; set; }

    }


}
