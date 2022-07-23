using Codx.Auth.Data.Contexts;
using Codx.Auth.Data.Entities.AspNet;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Codx.Auth.Extensions
{
    public static class DatabaseContext
    {
        public static IApplicationBuilder InitializeDb(this IApplicationBuilder app)
        {

            using (var scope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    var userDbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                    var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

                    userDbContext.Database.Migrate();
                    configurationDbContext.Database.Migrate();
                    persistedGrantDbContext.Database.Migrate();

                    // Execute Seed if there is no user data found
                    if (!userDbContext.Users.Any() && !userDbContext.Roles.Any())
                    {
                        SeedInitialAdminUser(userDbContext);
                        SeedInitialUser(userDbContext);
                    }                    

                }
                catch (Exception ex)
                {
                    // Add Logger here
                }
            }

            return app;
        }

        private static void SeedInitialAdminUser(UserDbContext userDbContext)
        {
            var defaultUserId = Guid.NewGuid();
            var defaultUsername = "administrator";
            var defaultUserEmail = "administrator@app.co";
            var defaultUserPassword = "AdminPass12345!";
            var defaultRoleId = Guid.NewGuid();
            var defaultRoleName = "Administrator";

            if (!userDbContext.Users.Any(o => o.UserName == defaultUsername))
            {
                ApplicationUser defaultUser = new ApplicationUser
                {
                    Id = defaultUserId,
                    UserName = defaultUsername,
                    NormalizedUserName = defaultUsername.ToUpper(),
                    Email = defaultUserEmail,
                    NormalizedEmail = defaultUserEmail.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
                defaultUser.PasswordHash = passwordHasher.HashPassword(defaultUser, defaultUserPassword);

                userDbContext.Users.Add(defaultUser);
                userDbContext.SaveChanges();
            }


            if (!userDbContext.Roles.Any(o => o.Name == defaultRoleName))
            {
                ApplicationRole defaultRole = new ApplicationRole
                {
                    Id = defaultRoleId,
                    Name = defaultRoleName,
                    NormalizedName = defaultRoleName.ToUpper()
                };

                userDbContext.Roles.Add(defaultRole);
                userDbContext.SaveChanges();
            }

            if (!userDbContext.UserRoles.Any(o => o.UserId == defaultUserId && o.RoleId == defaultRoleId))
            {
                ApplicationUserRole defaultUserRole = new ApplicationUserRole
                {
                    UserId = defaultUserId,
                    RoleId = defaultRoleId
                };

                userDbContext.UserRoles.Add(defaultUserRole);
                userDbContext.SaveChanges();
            }
                        
        }

        private static void SeedInitialUser(UserDbContext userDbContext)
        {
            var defaultUserId = Guid.NewGuid();
            var defaultUsername = "user";
            var defaultUserEmail = "user@app.co";
            var defaultUserPassword = "UserPass12345!";
            var defaultRoleId = Guid.NewGuid();
            var defaultRoleName = "User";

            if (!userDbContext.Users.Any(o => o.UserName == defaultUsername))
            {
                ApplicationUser defaultUser = new ApplicationUser
                {
                    Id = defaultUserId,
                    UserName = defaultUsername,
                    NormalizedUserName = defaultUsername.ToUpper(),
                    Email = defaultUserEmail,
                    NormalizedEmail = defaultUserEmail.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
                defaultUser.PasswordHash = passwordHasher.HashPassword(defaultUser, defaultUserPassword);

                userDbContext.Users.Add(defaultUser);
                userDbContext.SaveChanges();
            }


            if (!userDbContext.Roles.Any(o => o.Name == defaultRoleName))
            {
                ApplicationRole defaultRole = new ApplicationRole
                {
                    Id = defaultRoleId,
                    Name = defaultRoleName,
                    NormalizedName = defaultRoleName.ToUpper()
                };

                userDbContext.Roles.Add(defaultRole);
                userDbContext.SaveChanges();
            }

            if (!userDbContext.UserRoles.Any(o => o.UserId == defaultUserId && o.RoleId == defaultRoleId))
            {
                ApplicationUserRole defaultUserRole = new ApplicationUserRole
                {
                    UserId = defaultUserId,
                    RoleId = defaultRoleId
                };

                userDbContext.UserRoles.Add(defaultUserRole);
                userDbContext.SaveChanges();
            }

        }
    }
}
