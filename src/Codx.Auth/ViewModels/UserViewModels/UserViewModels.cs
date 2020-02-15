using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels.UserViewModels
{
    public class UserViewModels
    {

    }

    public class UserDetailsViewModel
    { 
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }

    public class UserAddViewModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class UserEditViewModel
    {

        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

    }
}
