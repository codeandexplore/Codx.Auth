using AutoMapper;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.Enterprise;
using Codx.Auth.Extensions;
using Codx.Auth.Infrastructure.Lifecycle;
using Codx.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Controllers
{
    [Authorize(Policy = "PlatformAdmin")]
    public class TenantsController : Controller
    {
        private readonly UserDbContext _context;
        protected readonly IMapper _mapper;
        private readonly ILifecycleCascadeService _cascade;

        public TenantsController(UserDbContext context, IMapper mapper, ILifecycleCascadeService cascade)
        {
            _context = context;
            _mapper = mapper;
            _cascade = cascade;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetTenantsTableData(string search, string sort, string order, int offset, int limit)
        {
            var tenants = _context.Tenants.Where(o => o.Status != LifecycleStatus.Tenant.Cancelled);
            var tenantList = tenants.OrderBy(o => o.Name).Skip(offset).Take(limit).ToList();
            return Json(new
            {
                total = tenants.Count(),
                rows = tenantList
            });
        }

        [HttpGet]
        public IActionResult Details(Guid id) 
        {
            var record = _context.Tenants.FirstOrDefault(o => o.Id == id && o.Status != LifecycleStatus.Tenant.Cancelled);

            if (record == null) return NotFound();

            var viewModel = _mapper.Map<TenantDetailsViewModel>(record);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(TenantAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.GetUserId();

                var record = _mapper.Map<Tenant>(viewModel);
                record.Status = LifecycleStatus.Tenant.Active;
                record.CreatedBy = userId;
                record.CreatedAt = DateTime.Now;
                      
                await _context.Tenants.AddAsync(record).ConfigureAwait(false);
                var result = await _context.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(Details), new { id = record.Id });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var record = _context.Tenants.FirstOrDefault(o => o.Id == id && o.Status != LifecycleStatus.Tenant.Cancelled);

            if (record == null) return NotFound();

            var viewModel = _mapper.Map<TenantEditViewModel>(record);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TenantEditViewModel viewModel)
        {
            var record = await _context.Tenants.FirstOrDefaultAsync(u => u.Id == viewModel.Id && u.Status != LifecycleStatus.Tenant.Cancelled);

            if (ModelState.IsValid && record != null)
            {
                var userId = User.GetUserId();

                record.Name = viewModel.Name;
                record.Email = viewModel.Email;
                record.Phone = viewModel.Phone;
                record.Address = viewModel.Address;
                record.Logo = viewModel.Logo;
                record.Theme = viewModel.Theme;
                record.Description = viewModel.Description;
                record.UpdatedAt = DateTime.Now;
                record.UpdatedBy = userId;

                var result = await _context.SaveChangesAsync().ConfigureAwait(false);

                if (result > 0)
                {
                    return RedirectToAction(nameof(Details), new { id = record.Id });
                }

                ModelState.AddModelError("", "Failed");
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var record = _context.Tenants.FirstOrDefault(o => o.Id == id && o.Status != LifecycleStatus.Tenant.Cancelled);

            if (record == null) return NotFound();

            var viewModel = _mapper.Map<TenantEditViewModel>(record);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TenantEditViewModel viewModel)
        {
            var record = _context.Tenants.FirstOrDefault(o => o.Id == viewModel.Id && o.Status != LifecycleStatus.Tenant.Cancelled);
            if (ModelState.IsValid && record != null)
            {
                var userId = User.GetUserId();

                record.Status = LifecycleStatus.Tenant.Cancelled;
                record.UpdatedAt = DateTime.UtcNow;
                record.UpdatedBy = userId;

                // Cascade: cancel all child companies, memberships, invitations, and sessions
                await _cascade.CancelTenantAsync(record.Id, userId, default);

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed");
            return View(viewModel);
        }
    }
}
