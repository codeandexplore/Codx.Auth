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
    public class ClientRedirectUrisController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public ClientRedirectUrisController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> RedirectUris(int id)
        {
            var client = await _dbContext.Clients.Include(i => i.RedirectUris).FirstOrDefaultAsync(u => u.Id == id);

            var viewmodel = new ClientRedirectUrisDetailsViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;
            viewmodel.Description = client.Description;

            foreach (var redirecturi in client.RedirectUris)
            {
                viewmodel.RedirectUris.Add(new ClientRedirectUriDetailsViewModel
                {
                    Id = redirecturi.Id,
                    RedirectUri = redirecturi.RedirectUri,
                });
            }

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Add(int clientid)
        {
            var client = await _dbContext.Clients.Include(i => i.RedirectUris).FirstOrDefaultAsync(u => u.Id == clientid);

            var viewmodel = new ClientRedirectUriAddViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientRedirectUriAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.Include(i => i.RedirectUris).FirstOrDefaultAsync(u => u.Id == viewmodel.ClientId);

                client.RedirectUris.Add(new ClientRedirectUri
                {
                    ClientId = viewmodel.ClientId,
                    RedirectUri = viewmodel.RedirectUri,                    
                });

                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(RedirectUris), new { id = viewmodel.ClientId });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }
    }
}
