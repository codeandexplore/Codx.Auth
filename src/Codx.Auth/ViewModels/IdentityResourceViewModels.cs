using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codx.Auth.ViewModels
{

    public class IdentityResourceDetailsViewModel : BaseIdentityResourceViewModel
    {
        public int Id { get; set; }
    }

    public class IdentityResourceAddViewModel : BaseIdentityResourceViewModel
    {

    }

    public class IdentityResourceEditViewModel : BaseIdentityResourceViewModel
    {
        public int Id { get; set; }
    }

    public class BaseIdentityResourceViewModel
    {

        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public bool NonEditable { get; set; }
    }
}
