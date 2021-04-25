using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class IdentityResourcePropertyDetailsViewModel : BaseIdentityResourcePropertyViewModel
    {
        public int Id { get; set; }
    }
    public class IdentityResourcePropertyAddViewModel : BaseIdentityResourcePropertyViewModel
    {
    }
    public class IdentityResourcePropertyEditViewModel: BaseIdentityResourcePropertyViewModel
    {
        public int Id { get; set; }
    }

    public class BaseIdentityResourcePropertyViewModel
    {        
        public int IdentityResourceId { get; set; }  
        [Required]
        [StringLength(250)]
        public string Key { get; set; }
        [Required]
        [StringLength(2000)]
        public string Value { get; set; }
    }
}
