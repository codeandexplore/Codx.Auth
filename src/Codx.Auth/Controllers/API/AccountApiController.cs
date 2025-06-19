using Codx.Auth.Services;
using Codx.Auth.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Codx.Auth.Controllers.API
{
    [Route("api/accounts")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountApiController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (result, user) = await _accountService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Registration successful" });
        }
    }
}
