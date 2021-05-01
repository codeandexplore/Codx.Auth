using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class RoleDetailsViewModel : BaseRoleViewModel
    {
        public Guid  Id { get; set; }
    }

    public class RoleAddViewModel : BaseRoleViewModel
    { 
    }

    public class RoleEditViewModel : BaseRoleViewModel
    {
        public Guid Id { get; set; }
    }
    public class BaseRoleViewModel
    {

        [Required]
        [StringLength(256)]
        public string Name { get; set; }
    }

}
