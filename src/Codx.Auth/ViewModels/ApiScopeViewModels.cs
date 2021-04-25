using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ApiScopeDetailsViewModel : BaseApiScopeViewModel
    {
        public int Id { get; set; }
    }
    public class ApiScopeAddViewModel : BaseApiScopeViewModel
    {
    }
    public class ApiScopeEditViewModel : BaseApiScopeViewModel
    {
        public int Id { get; set; }
    }

    public class BaseApiScopeViewModel
    {

        public bool Enabled { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string DisplayName { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
    }
}
