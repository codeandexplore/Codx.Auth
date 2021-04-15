using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ApiScopePropertyDetailsViewModel : BaseApiScopePropertyViewModel
    {
        public int Id { get; set; }
    }
    public class ApiScopePropertyAddViewModel : BaseApiScopePropertyViewModel
    {
    }
    public class ApiScopePropertyEditViewModel: BaseApiScopePropertyViewModel
    {
        public int Id { get; set; }
    }

    public class BaseApiScopePropertyViewModel
    {        
        public int ScopeId { get; set; }  
        [Required]
        [StringLength(250)]
        public string Key { get; set; }
        [Required]
        [StringLength(2000)]
        public string Value { get; set; }
    }
}
