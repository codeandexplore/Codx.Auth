using AutoMapper;
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
    public class ApiResourcesController : Controller
    {
        protected readonly ConfigurationDbContext _dbContext;
        protected readonly IMapper _mapper;
        public ApiResourcesController(ConfigurationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetApiResourcesTableData(string search, string sort, string order, int offset, int limit)
        {
            var apiResources = _dbContext.ApiResources.Skip(offset).Take(limit).ToList();
            var viewModel = apiResources.Select(apires => new ApiResourceDetailsViewModel
            {
                Id = apires.Id,
                Name = apires.Name,
                DisplayName = apires.DisplayName,
            }).ToList();

            return Json(new { 
                total = _dbContext.ApiResources.Count(),
                rows = viewModel
            });
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var record = _dbContext.ApiResources.FirstOrDefault(o => o.Id == id);

            var viewmodel = _mapper.Map<ApiResourceDetailsViewModel>(record);

            return View(viewmodel);
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ApiResourceAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var record = _mapper.Map<ApiResource>(viewModel);

                record.Enabled = true;
                record.ShowInDiscoveryDocument = true;
                record.Created = DateTime.UtcNow;

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
            var record = _dbContext.ApiResources.FirstOrDefault(o => o.Id == id);

            var viewmodel = _mapper.Map<ApiResourceEditViewModel>(record);

            return View(viewmodel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApiResourceEditViewModel viewmodel)
        {
            var isRecordFound = _dbContext.ApiResources.Any(o => o.Id == viewmodel.Id);

            if (ModelState.IsValid && isRecordFound)
            {                
                var record = _mapper.Map<ApiResource>(viewmodel);
                record.Updated = DateTime.UtcNow;

                _dbContext.Update(record);
                var result = await _dbContext.SaveChangesAsync();

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
            var record = _dbContext.ApiResources.FirstOrDefault(o => o.Id == id);

            var viewmodel = _mapper.Map<ApiResourceEditViewModel>(record);

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ApiResourceEditViewModel viewmodel)
        {
            var isRecordFound = _dbContext.ApiResources.Any(o => o.Id == viewmodel.Id);

            if (ModelState.IsValid && isRecordFound)
            {
                var record = _mapper.Map<ApiResource>(viewmodel);
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
