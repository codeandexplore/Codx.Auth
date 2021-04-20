using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{

    public class ClientGrantTypeDetailsViewModel : BaseGrantTypeClaimViewModel
    {
        public int Id { get; set; }
    }


    public class ClientGrantTypeAddViewModel : BaseGrantTypeClaimViewModel
    {

    }

    public class ClientGrantTypeEditViewModel : BaseGrantTypeClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseGrantTypeClaimViewModel
    {
        public int ClientId { get; set; }

        [Required]
        [StringLength(250)]
        public string GrantType { get; set; }
    }


}
