using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Codx.Auth.Controllers.API
{
    [Route("api/v1/tenants")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class TenantApiController : ControllerBase
    {
    }
}
