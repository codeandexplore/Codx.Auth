using Codx.Auth.ViewModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers
{
    public class ClientScopesController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public ClientScopesController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Scopes(int id)
        {
            var client = await _dbContext.Clients.Include(i => i.AllowedScopes).FirstOrDefaultAsync(u => u.Id == id);

            var viewmodel = new ClientScopesDetailsViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;
            viewmodel.Description = client.Description;

            foreach (var Scope in client.AllowedScopes)
            {
                viewmodel.Scopes.Add(new ClientScopeDetailsViewModel
                {
                    Id = Scope.Id,
                    Scope = Scope.Scope,
                });
            }

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Add(int clientid)
        {
            var client = await _dbContext.Clients.Include(i => i.AllowedScopes).FirstOrDefaultAsync(u => u.Id == clientid);

            var viewmodel = new ClientScopeAddViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientScopeAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.Include(i => i.AllowedScopes).FirstOrDefaultAsync(u => u.Id == viewmodel.ClientId);

                client.AllowedScopes.Add(new ClientScope
                {
                    ClientId = viewmodel.ClientId,
                    Scope = viewmodel.Scope,                    
                });

                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(Scopes), new { id = viewmodel.ClientId });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }
    }
}
