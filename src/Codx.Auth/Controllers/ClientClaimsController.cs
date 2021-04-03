using Codx.Auth.ViewModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class ClientClaimsController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public ClientClaimsController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Claims(int id)
        {
            var client = await _dbContext.Clients.Include(i => i.Claims).FirstOrDefaultAsync(u => u.Id == id);

            var viewmodel = new ClientClaimsDetailsViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;
            viewmodel.Description = client.Description;

            foreach (var claim in client.Claims)
            {
                viewmodel.Claims.Add(new ClientClaimDetailsViewModel
                {
                    Id = claim.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value,
                });
            }

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Add(int clientid)
        {
            var client = await _dbContext.Clients.Include(i => i.Claims).FirstOrDefaultAsync(u => u.Id == clientid);

            var viewmodel = new ClientClaimAddViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientClaimAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.Include(i => i.Claims).FirstOrDefaultAsync(u => u.Id == viewmodel.ClientId);

                client.Claims.Add(new ClientClaim
                {
                    ClientId = viewmodel.ClientId,
                    Type = viewmodel.ClaimType,
                    Value = viewmodel.ClaimValue
                });

                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(Claims), new { id = viewmodel.ClientId });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }

    }
}
