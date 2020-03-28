using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.ViewModels.UserClaimViewModels
{
    public class UserClaimViewModels
    {
    }

    public class ClaimDetailsViewModel 
    {
        public Guid UserId { get; set; }

        public int ClaimId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    public class UserClaimsDetailsViewModel
    {
        public UserClaimsDetailsViewModel()
        {
            Claims = new List<ClaimDetailsViewModel>();
        }

        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public List<ClaimDetailsViewModel> Claims { get; set; }
    }


    public class UserClaimAddViewModel
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    public class UserClaimDeleteViewModel
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }

        public int ClaimId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }


}
