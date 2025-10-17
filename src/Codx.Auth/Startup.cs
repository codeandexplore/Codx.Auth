using System;
using System.Reflection;
using Codx.Auth.Configuration;
using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Extensions;
using Codx.Auth.Mappings;
using Codx.Auth.Services;
using Codx.Auth.Services.Interfaces;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Codx.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddAutoMapper(typeof(ApplicationProfile));

            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString));

           services.AddTransient<IFilterService, FilterService>();
            
            services.AddTransient<IAccountService, AccountService>();

            services.AddAspNetIdentity();
                       
            services.AddControllersWithViews();

            // Configure data protection for Docker
            //services.AddDataProtection()
            //    .PersistKeysToFileSystem(new System.IO.DirectoryInfo("/app/keys"))
            //    .SetApplicationName("Codx.Auth");

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            
            services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<CustomProfileService>();

            services.AddDbContext<IdentityServerDbContext>();

            services.AddScoped<ITenantResolver, TenantResolver>();

            // Configure external authentication providers
            var externalAuthConfig = new ExternalAuthConfiguration();
            Configuration.GetSection("Authentication").Bind(externalAuthConfig);

            var authBuilder = services.AddAuthentication()
                .AddCookie(options => {
                    options.ExpireTimeSpan = new TimeSpan(0, 15, 0);
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.IsEssential = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });

            // Google Authentication
            if (externalAuthConfig.Google.IsConfigured)
            {
                authBuilder.AddGoogle(options =>
                {
                    options.ClientId = externalAuthConfig.Google.ClientId;
                    options.ClientSecret = externalAuthConfig.Google.ClientSecret;
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                });
            }

            // Facebook Authentication
            if (externalAuthConfig.Facebook.IsConfigured)
            {
                authBuilder.AddFacebook(options =>
                {
                    options.AppId = externalAuthConfig.Facebook.AppId;
                    options.AppSecret = externalAuthConfig.Facebook.AppSecret;
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                });
            }

            // Microsoft Authentication
            if (externalAuthConfig.Microsoft.IsConfigured)
            {
                authBuilder.AddMicrosoftAccount(options =>
                {
                    options.ClientId = externalAuthConfig.Microsoft.ClientId;
                    options.ClientSecret = externalAuthConfig.Microsoft.ClientSecret;
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                });
            }

            // Configure cookie policy for Docker compatibility
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.Secure = CookieSecurePolicy.SameAsRequest;
            });

            services.AddLocalApiAuthentication();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IdentityServerAdmin", policy => policy.RequireRole("Administrator"));
                options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            // Add CORS services
            var allowedOrigins = Configuration["AllowedOrigins"]?.Split(',') ?? Array.Empty<string>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.InitializeDb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure forwarded headers for Docker/proxy scenarios
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
                RequireHeaderSymmetry = false,
                ForwardLimit = null
            };
            
            // Configure known networks and proxies for Docker
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            forwardedHeadersOptions.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(
                System.Net.IPAddress.Parse("0.0.0.0"), 0));
            
            app.UseForwardedHeaders(forwardedHeadersOptions);

            // Only redirect to HTTPS in development environment
            // In production Docker, the reverse proxy should handle HTTPS
            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            
            app.UseStaticFiles();

            // Add cookie policy middleware
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            // Use CORS middleware
            app.UseCors("AllowSpecificOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
