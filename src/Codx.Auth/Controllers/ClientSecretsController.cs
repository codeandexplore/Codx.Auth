using Codx.Auth.ViewModels;
using IdentityModel;
using IdentityServer4;
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
    public class ClientSecretsController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public ClientSecretsController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Secrets(int id)
        {
            var client = await _dbContext.Clients.Include(i => i.ClientSecrets).FirstOrDefaultAsync(u => u.Id == id);

            var viewmodel = new ClientSecretsDetailsViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;
            viewmodel.Description = client.Description;

            foreach (var Secret in client.ClientSecrets)
            {
                viewmodel.Secrets.Add(new ClientSecretDetailsViewModel
                {
                    Id = Secret.Id,
                    Type = Secret.Type,
                    Value = Secret.Value,
                    Description = Secret.Description,
                });
            }

            return View(viewmodel);
        }


        [HttpGet]
        public async Task<IActionResult> Add(int clientid)
        {
            var client = await _dbContext.Clients.Include(i => i.ClientSecrets).FirstOrDefaultAsync(u => u.Id == clientid);

            var viewmodel = new ClientSecretAddViewModel();

            viewmodel.ClientId = client.Id;
            viewmodel.ClientIdString = client.ClientId;
            viewmodel.ClientName = client.ClientName;

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientSecretAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.Include(i => i.ClientSecrets).FirstOrDefaultAsync(u => u.Id == viewmodel.ClientId);

                client.ClientSecrets.Add(new ClientSecret
                {
                    ClientId = viewmodel.ClientId,
                    Type = IdentityServerConstants.SecretTypes.SharedSecret,
                    Value = new IdentityServer4.Models.Secret(viewmodel.Value.ToSha256()).Value,
                    Description = viewmodel.Description,
                    Created = DateTime.UtcNow
                });

                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(Secrets), new { id = viewmodel.ClientId });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }

    }
}
