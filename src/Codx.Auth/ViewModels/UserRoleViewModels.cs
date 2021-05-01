using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels
{
    public class UserRoleDetailsViewModel : BaseUserRoleViewModel
    {
        public string Role { get; set; }
    }

    public class UserRoleAddViewModel : BaseUserRoleViewModel
    {
        public UserRoleAddViewModel()
        {
            Roles = new List<SelectListItem>();
        }

        public List<SelectListItem> Roles { get; set; }
    }

    public class UserRoleEditViewModel : BaseUserRoleViewModel
    {
        public UserRoleEditViewModel()
        {
            Roles = new List<SelectListItem>();
        }

        [Required]
        public Guid OldRoleId { get; set; }
        public string Role { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }

    public class BaseUserRoleViewModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }
    }

}
