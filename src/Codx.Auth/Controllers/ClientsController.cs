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

        // Edit Client
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _dbContext.Clients.FirstOrDefaultAsync(u => u.Id == id);

            var viewModel = new ClientEditViewModel
            {
                Id = record.Id,
                ClientId = record.ClientId,
                ClientName = record.ClientName,
                Description = record.Description,

                ClientUri = record.ClientUri,
                LogoUri = record.LogoUri,
                Enabled = record.Enabled,
                EnableLocalLogin = record.EnableLocalLogin,
                AllowOfflineAccess = record.AllowOfflineAccess,
                FrontChannelLogoutUri = record.FrontChannelLogoutUri,
                FrontChannelLogoutSessionRequired = record.FrontChannelLogoutSessionRequired,
                BackChannelLogoutUri = record.BackChannelLogoutUri,
                BackChannelLogoutSessionRequired = record.BackChannelLogoutSessionRequired,


                AccessTokenLifetime = record.AccessTokenLifetime,
                AuthorizationCodeLifetime = record.AuthorizationCodeLifetime,
                ConsentLifetime = record.ConsentLifetime,
                AbsoluteRefreshTokenLifetime = record.AbsoluteRefreshTokenLifetime,
                SlidingRefreshTokenLifetime = record.SlidingRefreshTokenLifetime,
                UserSsoLifetime = record.UserSsoLifetime,
                DeviceCodeLifetime = record.DeviceCodeLifetime,
                IdentityTokenLifetime = record.IdentityTokenLifetime,

                AccessTokenType = record.AccessTokenType,
                UpdateAccessTokenClaimsOnRefresh = record.UpdateAccessTokenClaimsOnRefresh,
                RefreshTokenUsage = record.RefreshTokenUsage,
                RefreshTokenExpiration = record.RefreshTokenExpiration,
                AllowAccessTokensViaBrowser = record.AllowAccessTokensViaBrowser,

                IncludeJwtId = record.IncludeJwtId,
                ClientClaimsPrefix = record.ClientClaimsPrefix,
                AlwaysIncludeUserClaimsInIdToken = record.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = record.AlwaysSendClientClaims,
                PairWiseSubjectSalt = record.PairWiseSubjectSalt,
                UserCodeType = record.UserCodeType,
                AllowedIdentityTokenSigningAlgorithms = record.AllowedIdentityTokenSigningAlgorithms,
                ProtocolType = record.ProtocolType,
                RequireClientSecret = record.RequireClientSecret,
                RequireConsent = record.RequireConsent,
                AllowRememberConsent = record.AllowRememberConsent,
                RequirePkce = record.RequirePkce,
                AllowPlainTextPkce = record.AllowPlainTextPkce,
                RequireRequestObject = record.RequireRequestObject,
                NonEditable = record.NonEditable
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ClientEditViewModel viewmodel)
        {
            var record = await _dbContext.Clients.FirstOrDefaultAsync(u => u.Id == viewmodel.Id);

            if (ModelState.IsValid)
            {
                record.ClientId = record.ClientId;
                record.ClientName = record.ClientName;
                record.Description = record.Description;

                record.ClientUri = record.ClientUri;
                record.LogoUri = record.LogoUri;
                record.Enabled = record.Enabled;
                record.EnableLocalLogin = record.EnableLocalLogin;
                record.AllowOfflineAccess = record.AllowOfflineAccess;
                record.FrontChannelLogoutUri = record.FrontChannelLogoutUri;
                record.FrontChannelLogoutSessionRequired = record.FrontChannelLogoutSessionRequired;
                record.BackChannelLogoutUri = record.BackChannelLogoutUri;
                record.BackChannelLogoutSessionRequired = record.BackChannelLogoutSessionRequired;


                record.AccessTokenLifetime = record.AccessTokenLifetime;
                record.AuthorizationCodeLifetime = record.AuthorizationCodeLifetime;
                record.ConsentLifetime = record.ConsentLifetime;
                record.AbsoluteRefreshTokenLifetime = record.AbsoluteRefreshTokenLifetime;
                record.SlidingRefreshTokenLifetime = record.SlidingRefreshTokenLifetime;
                record.UserSsoLifetime = record.UserSsoLifetime;
                record.DeviceCodeLifetime = record.DeviceCodeLifetime;
                record.IdentityTokenLifetime = record.IdentityTokenLifetime;

                record.AccessTokenType = record.AccessTokenType;
                record.UpdateAccessTokenClaimsOnRefresh = record.UpdateAccessTokenClaimsOnRefresh;
                record.RefreshTokenUsage = record.RefreshTokenUsage;
                record.RefreshTokenExpiration = record.RefreshTokenExpiration;
                record.AllowAccessTokensViaBrowser = record.AllowAccessTokensViaBrowser;

                record.IncludeJwtId = record.IncludeJwtId;
                record.ClientClaimsPrefix = record.ClientClaimsPrefix;
                record.AlwaysIncludeUserClaimsInIdToken = record.AlwaysIncludeUserClaimsInIdToken;
                record.AlwaysSendClientClaims = record.AlwaysSendClientClaims;
                record.PairWiseSubjectSalt = record.PairWiseSubjectSalt;
                record.UserCodeType = record.UserCodeType;
                record.AllowedIdentityTokenSigningAlgorithms = record.AllowedIdentityTokenSigningAlgorithms;
                record.ProtocolType = record.ProtocolType;
                record.RequireClientSecret = record.RequireClientSecret;
                record.RequireConsent = record.RequireConsent;
                record.AllowRememberConsent = record.AllowRememberConsent;
                record.RequirePkce = record.RequirePkce;
                record.AllowPlainTextPkce = record.AllowPlainTextPkce;
                record.RequireRequestObject = record.RequireRequestObject;
                record.NonEditable = record.NonEditable;

                var result = await _dbContext.SaveChangesAsync().ConfigureAwait(false);

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
