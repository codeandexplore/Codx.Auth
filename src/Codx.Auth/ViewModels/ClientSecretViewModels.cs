using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ClientSecretViewModels
    {
    }

    public class ClientSecretDetailsViewModel 
    {
        public int Id { get; set; }

        public string Type { get; set; }
        public string Value { get; set; }

        public string Description { get; set; }
    }

    public class ClientSecretsDetailsViewModel
    {
        public ClientSecretsDetailsViewModel()
        {
            Secrets = new List<ClientSecretDetailsViewModel>();
        }

        public int ClientId { get; set; }
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public List<ClientSecretDetailsViewModel> Secrets { get; set; }
    }


    public class ClientSecretAddViewModel : BaseClientSecretViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class ClientSecretEditViewModel : BaseClientSecretViewModel
    {
        public string ClientIdString { get; set; }
        public string ClientName { get; set; }
    }

    public class BaseClientSecretViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Required]
        [StringLength(250)]
        public string Value { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }
    }


}
