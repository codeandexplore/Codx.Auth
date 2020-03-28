using Codx.Auth.ViewModels.RoleViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels.UserRoleViewModels
{
    public class UserRoleViewModels
    {
    }

    public class UserRolesDetailsViewModel 
    {
        public UserRolesDetailsViewModel()
        {
            Roles = new List<RoleDetailsViewModel>();
        }

        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public List<RoleDetailsViewModel> Roles { get; set; }
    }

    public class UserRoleAddViewModel
    {
        public UserRoleAddViewModel()
        {
            Roles = new List<SelectListItem>();
        }

        [Required]
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        [Required]
        public Guid RoleId { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }

    public class UserRoleDeleteViewModel
    {
        [Required]
        public Guid UserId { get; set; }
        public string Username { get; set; }

        [Required]
        public Guid RoleId { get; set; }
        public string Rolename { get; set; }
    }

}
