using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ApiResourceSecretDetailsViewModel : BaseApiResourceSecretViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
    public class ApiResourceSecretAddViewModel : BaseApiResourceSecretViewModel
    {
        [Required]
        [StringLength(200)]
        public string NewSecret { get; set; }
    }
    public class ApiResourceSecretEditViewModel : BaseApiResourceSecretViewModel
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string NewSecret { get; set; }
        public string Value { get; set; }

    }

    public class BaseApiResourceSecretViewModel
    {        
        public int ApiResourceId { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
       
        public DateTime? Expiration { get; set; }
        [Required]
        [StringLength(250)]
        public string Type { get; set; }
    }
}
