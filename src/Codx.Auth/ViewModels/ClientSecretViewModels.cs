using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientSecretDetailsViewModel : BaseClientSecretViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class ClientSecretAddViewModel : BaseClientSecretViewModel
    {
        [Required]
        [StringLength(200)]
        public string NewSecret { get; set; }
    }

    public class ClientSecretEditViewModel : BaseClientSecretViewModel
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string NewSecret { get; set; }
        public string Value { get; set; }
    }

    public class BaseClientSecretViewModel
    {
        public int ClientId { get; set; }
        [Required]
        [StringLength(250)]
        public string Type { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }
        public DateTime? Expiration { get; set; }
    }


}
