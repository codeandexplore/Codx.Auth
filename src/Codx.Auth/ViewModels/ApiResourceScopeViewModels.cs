using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ApiResourceScopeDetailsViewModel : BaseApiResourceScopeViewModel
    {
        public int Id { get; set; }
    }
    public class ApiResourceScopeAddViewModel : BaseApiResourceScopeViewModel
    {
    }
    public class ApiResourceScopeEditViewModel : BaseApiResourceScopeViewModel
    {
        public int Id { get; set; }
    }

    public class BaseApiResourceScopeViewModel
    {
        
        public int ApiResourceId { get; set; }  
        [Required]
        [StringLength(200)]
        public string Scope { get; set; }
    }
}
