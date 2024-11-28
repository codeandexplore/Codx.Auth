using System;
using System.Security.Claims;

namespace Codx.Auth.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        private const string UserFirstNameClaimType = "given_name";
        private const string UserLastNameClaimType = "family_name";

        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var claim = principal.FindFirst("sub");
            return claim == null ? Guid.Empty : new Guid(claim.Value);
        }

        public static string GetUserFirstName(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var claim = principal.FindFirst(UserFirstNameClaimType);
            return claim == null ? string.Empty : claim.Value;
        }

        public static string GetUserLastName(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var claim = principal.FindFirst(UserLastNameClaimType);
            return claim == null ? string.Empty : claim.Value;
        }

        public static string GetUserFullName(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var firstName = principal.GetUserFirstName();
            var lastName = principal.GetUserLastName();
            return $"{firstName} {lastName}";
        }
    }
}
