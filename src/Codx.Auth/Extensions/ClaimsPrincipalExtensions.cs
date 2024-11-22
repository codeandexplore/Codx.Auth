using System;
using System.Security.Claims;

namespace Codx.Auth.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var claim = principal.FindFirst("sub");
            return claim == null ? Guid.Empty : new Guid(claim.Value);
        }
    }
}
