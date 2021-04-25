using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ApiScopeClaimDetailsViewModel : BaseApiScopeClaimViewModel
    {
        public int Id { get; set; }
    }
    public class ApiScopeClaimAddViewModel : BaseApiScopeClaimViewModel
    {
    }
    public class ApiScopeClaimEditViewModel: BaseApiScopeClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseApiScopeClaimViewModel
    {
        
        public int ScopeId { get; set; }  
        [Required]
        [StringLength(200)]
        public string Type { get; set; }
    }
}
