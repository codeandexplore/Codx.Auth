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
    public class ClientPostLogoutRedirectUrisController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public ClientPostLogoutRedirectUrisController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> PostLogoutRedirectUris(int id)
        {
            var client = await _dbContext.Clients.Include(i => i.PostLogoutRedirectUris).FirstOrDefaultAsync(u => u.Id == id);

            var viewmodel = new ClientPostLogoutRedirectUrisDetailsViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;
            viewmodel.Description = client.Description;

            foreach (var redirecturi in client.PostLogoutRedirectUris)
            {
                viewmodel.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUriDetailsViewModel
                {
                    Id = redirecturi.Id,
                    PostLogoutRedirectUri = redirecturi.PostLogoutRedirectUri,
                });
            }

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Add(int clientid)
        {
            var client = await _dbContext.Clients.Include(i => i.PostLogoutRedirectUris).FirstOrDefaultAsync(u => u.Id == clientid);

            var viewmodel = new ClientPostLogoutRedirectUriAddViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientPostLogoutRedirectUriAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.Include(i => i.PostLogoutRedirectUris).FirstOrDefaultAsync(u => u.Id == viewmodel.ClientId);

                client.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri
                {
                    ClientId = viewmodel.ClientId,
                    PostLogoutRedirectUri = viewmodel.PostLogoutRedirectUri,                    
                });

                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(PostLogoutRedirectUris), new { id = viewmodel.ClientId });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }
    }
}
