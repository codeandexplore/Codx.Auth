using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientViewModels
    {
    }

    public class ClientDetailsViewModel
    {  
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
    }

    public class ClientAddViewModel : BaseClientViewModel
    {

    }

    public class ClientEditViewModel : BaseClientViewModel
    {

    }

    public class BaseClientViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ClientId { get; set; }
        [StringLength(200)]
        public string ClientName { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
