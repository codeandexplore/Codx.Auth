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
    public class IdentityResourceClaimsController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public IdentityResourceClaimsController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> IdentityResourceClaims(int id)
        {
            var identityresource = _dbContext.IdentityResources.Include(o => o.UserClaims).FirstOrDefault(o => o.Id == id);

            var viewmodel = new IdentityResourceDetailsViewModel();

            viewmodel.Id = identityresource.Id;
            viewmodel.Name = identityresource.Name;
            viewmodel.DisplayName = identityresource.DisplayName;

          

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            var identityresource = _dbContext.IdentityResources.Include(o => o.UserClaims).FirstOrDefault(o => o.Id == id);

            var viewmodel = new IdentityResourceClaimAddViewModel();

            viewmodel.IdentityResourceId = identityresource.Id;
            viewmodel.Name = identityresource.Name;
            viewmodel.DisplayName = identityresource.DisplayName;

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(IdentityResourceClaimAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var identityresource = _dbContext.IdentityResources.Include(o => o.UserClaims).FirstOrDefault(o => o.Id == viewmodel.IdentityResourceId);

                identityresource.UserClaims.Add(new IdentityResourceClaim {
                    IdentityResourceId = viewmodel.IdentityResourceId,
                    Type = viewmodel.Type,
                });

                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(IdentityResourceClaims), new { id = viewmodel.IdentityResourceId });
                }
                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }

    }
}
