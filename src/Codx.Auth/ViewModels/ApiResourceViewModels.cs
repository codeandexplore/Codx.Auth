using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ApiResourceDetailsViewModel : BaseApiResourceViewModel
    {
    }
    public class ApiResourceAddViewModel : BaseApiResourceViewModel
    {
    }
    public class ApiResourceEditViewModel : BaseApiResourceViewModel
    {
    }

    public class BaseApiResourceViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(100)]
        public string AllowedAccessTokenSigningAlgorithms { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public bool NonEditable { get; set; }
    }
}
