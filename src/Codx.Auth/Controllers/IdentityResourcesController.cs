using Codx.Auth.ViewModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers
{
    [Authorize]
    public class IdentityResourcesController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        public IdentityResourcesController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var identityResources = _dbContext.IdentityResources.ToList();

            var viewModel = identityResources.Select(idres => new IdentityResourceDetailsViewModel
            {
                Id = idres.Id,
                Name = idres.Name,
                DisplayName = idres.DisplayName,
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IdentityResourceAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var record = new IdentityResource {
                    Name = viewModel.Name,
                    DisplayName = viewModel.DisplayName,
                    Enabled = true,
                    ShowInDiscoveryDocument = true,
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

        public async Task<IActionResult> Edit(int id)
        {
            var record = _dbContext.IdentityResources.FirstOrDefault(o => o.Id == id);

            var viewModel = new IdentityResourceEditViewModel
            {
                Id = record.Id,
                Name = record.Name,
                DisplayName = record.DisplayName,
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(IdentityResourceEditViewModel viewmodel)
        {
            var record = _dbContext.IdentityResources.FirstOrDefault(o => o.Id == viewmodel.Id);

            if (ModelState.IsValid)
            {
                record.Name = viewmodel.Name;
                record.DisplayName = viewmodel.DisplayName;

                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }
    }
}
