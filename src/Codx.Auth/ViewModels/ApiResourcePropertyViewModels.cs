using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class ApiResourcePropertyDetailsViewModel : BaseApiResourcePropertyViewModel
    {
        public int Id { get; set; }
    }
    public class ApiResourcePropertyAddViewModel : BaseApiResourcePropertyViewModel
    {
    }
    public class ApiResourcePropertyEditViewModel: BaseApiResourcePropertyViewModel
    {
        public int Id { get; set; }
    }

    public class BaseApiResourcePropertyViewModel
    {        
        public int ApiResourceId { get; set; }  
        [Required]
        [StringLength(250)]
        public string Key { get; set; }
        [Required]
        [StringLength(2000)]
        public string Value { get; set; }
    }
}
