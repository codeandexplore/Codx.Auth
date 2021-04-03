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
    public class ClientGrantTypesController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public ClientGrantTypesController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> GrantTypes(int id)
        {
            var client = await _dbContext.Clients.Include(i => i.AllowedGrantTypes).FirstOrDefaultAsync(u => u.Id == id);

            var viewmodel = new ClientGrantTypesDetailsViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;
            viewmodel.Description = client.Description;

            foreach (var granttype in client.AllowedGrantTypes)
            {
                viewmodel.GrantTypes.Add(new ClientGrantTypeDetailsViewModel
                {
                    Id = granttype.Id,
                    GrantType = granttype.GrantType,
                });
            }

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Add(int clientid)
        {
            var client = await _dbContext.Clients.Include(i => i.AllowedGrantTypes).FirstOrDefaultAsync(u => u.Id == clientid);

            var viewmodel = new ClientGrantTypeAddViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientGrantTypeAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.Include(i => i.AllowedGrantTypes).FirstOrDefaultAsync(u => u.Id == viewmodel.ClientId);

                client.AllowedGrantTypes.Add(new ClientGrantType
                {
                    ClientId = viewmodel.ClientId,
                    GrantType = viewmodel.GrantType,                    
                });

                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(GrantTypes), new { id = viewmodel.ClientId });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }
    }
}
