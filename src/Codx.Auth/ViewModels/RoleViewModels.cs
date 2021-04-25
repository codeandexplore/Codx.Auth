using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class RoleViewModels
    {
    }

    public class RoleDetailsViewModel 
    {
        public Guid  Id { get; set; }

        public string Name { get; set; }
    }

    public class RoleAddViewModel
    { 
        [Required]
        public string Name { get; set; }
    }

    public class RoleEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }


}
