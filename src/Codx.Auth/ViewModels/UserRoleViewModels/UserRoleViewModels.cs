using Codx.Auth.ViewModels.RoleViewModels;
using System;
using System.Collections.Generic;
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
}
