using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientClaimDetailsViewModel : BaseClientClaimViewModel
    {
        public int Id { get; set; }
    }


    public class ClientClaimAddViewModel : BaseClientClaimViewModel
    {
    }

    public class ClientClaimEditViewModel : BaseClientClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseClientClaimViewModel
    {

        public int ClientId { get; set; }

        [Required]
        [StringLength(250)]
        public string Type { get; set; }

        [Required]
        [StringLength(250)]
        public string Value { get; set; }
    }


}
