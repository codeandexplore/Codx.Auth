using AutoMapper;
using Codx.Auth.Data.Contexts;
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
    public class ClientsController : Controller
    {
        protected readonly IdentityServerDbContext _dbContext;
        protected readonly IMapper _mapper;
        public ClientsController(IdentityServerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetClientsTableData(string search, string sort, string order, int offset, int limit)
        {
            var apiResources = _dbContext.Clients.Skip(offset).Take(limit).ToList();
            var viewModel = apiResources.Select(apires => new ClientDetailsViewModel
            {
                Id = apires.Id,
                ClientId = apires.ClientId,
                ClientName = apires.ClientName,
            }).ToList();

            return Json(new
            {
                total = _dbContext.Clients.Count(),
                rows = viewModel
            });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var record = _dbContext.Clients.FirstOrDefault(o => o.Id == id);

            var viewmodel = _mapper.Map<ClientDetailsViewModel>(record);

            return View(viewmodel);
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

        // Edit Client
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _dbContext.Clients.FirstOrDefaultAsync(u => u.Id == id);

            var viewmodel = _mapper.Map<ClientEditViewModel>(record);

            return View(viewmodel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ClientEditViewModel viewmodel)
        {
            var isRecordFound = await _dbContext.Clients.AnyAsync(u => u.Id == viewmodel.Id);

            if (ModelState.IsValid && isRecordFound)
            {                
                var record = _mapper.Map<Client>(viewmodel);

                _dbContext.Update(record);
                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewmodel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var record = _dbContext.Clients.FirstOrDefault(o => o.Id == id);

            var viewmodel = _mapper.Map<ClientEditViewModel>(record);

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ClientEditViewModel viewmodel)
        {
            var isRecordFound = _dbContext.Clients.Any(o => o.Id == viewmodel.Id);

            if (ModelState.IsValid && isRecordFound)
            {
                var record = _mapper.Map<Client>(viewmodel);
                _dbContext.Remove(record);
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
