using Codx.Auth.ViewModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers
{
    public class ClientsController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public ClientsController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var clients = _dbContext.Clients.ToList();

            var viewModel = clients.Select(client => new ClientDetailsViewModel
            {
                Id = client.Id,
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                Description = client.Description,
            }).ToList();

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var record = new Client
                {
                    ClientId = viewModel.ClientId,
                    ClientName = viewModel.ClientName,
                    Description = viewModel.Description,
                    Enabled = true,
                    Created = DateTime.UtcNow,
                };

                await _dbContext.AddAsync(record).ConfigureAwait(false);
                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }


                ModelState.AddModelError("", "Failed");

            }

            return View(viewModel);
        }
    }
}
