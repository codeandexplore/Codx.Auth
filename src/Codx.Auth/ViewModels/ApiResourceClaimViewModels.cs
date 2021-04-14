using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ApiResourceClaimDetailsViewModel : BaseApiResourceClaimViewModel
    {
        public int Id { get; set; }
    }
    public class ApiResourceClaimAddViewModel : BaseApiResourceClaimViewModel
    {
    }
    public class ApiResourceClaimEditViewModel: BaseApiResourceClaimViewModel
    {
        public int Id { get; set; }
    }

    public class BaseApiResourceClaimViewModel
    {
        
        public int ApiResourceId { get; set; }  
        [Required]
        [StringLength(200)]
        public string Type { get; set; }
    }
}
