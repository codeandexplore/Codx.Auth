using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{

    public class ClientCorsOriginDetailsViewModel : BaseCorsOriginClaimViewModel
    {
        public int Id { get; set; }
    }


    public class ClientCorsOriginAddViewModel : BaseCorsOriginClaimViewModel
    {

    }

    public class ClientCorsOriginEditViewModel : BaseCorsOriginClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseCorsOriginClaimViewModel
    {
        public int ClientId { get; set; }

        [Required]
        [StringLength(150)]
        public string Origin { get; set; }
    }


}
