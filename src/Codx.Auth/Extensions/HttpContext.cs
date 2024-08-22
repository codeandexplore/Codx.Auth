using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Codx.Auth.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Determines if the authentication scheme support signout.
        /// </summary>
        internal static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
        {
            var provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            var handler = await provider.GetHandlerAsync(context, scheme);
            return (handler is IAuthenticationSignOutHandler);
        }
    }
}
