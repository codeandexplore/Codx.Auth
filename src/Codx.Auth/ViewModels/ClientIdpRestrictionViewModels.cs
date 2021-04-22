using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientIdpRestrictionDetailsViewModel : BaseClientIdpRestrictionViewModel
    {
        public int Id { get; set; }
    }


    public class ClientIdpRestrictionAddViewModel : BaseClientIdpRestrictionViewModel
    {
    }

    public class ClientIdpRestrictionEditViewModel : BaseClientIdpRestrictionViewModel
    {
        public int Id { get; set; }
    }

    public class BaseClientIdpRestrictionViewModel
    {

        public int ClientId { get; set; }

        [Required]
        [StringLength(200)]
        public string Provider { get; set; }
    }


}
