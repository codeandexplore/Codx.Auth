using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Data.Contexts
{
    public class UserDbContext : IdentityDbContext
        <ApplicationUser, 
        ApplicationRole, 
        Guid,
        ApplicationUserClaim,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b => {
                b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });

            builder.Entity<ApplicationUser>(b => {
                b.HasMany(e => e.UserClaims)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });

            builder.Entity<ApplicationRole>(b => {
                b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            });

            builder.Entity<UserCompany>()
                .HasKey(uc => new { uc.UserId, uc.CompanyId });

            builder.Entity<UserCompany>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCompanies)
                .HasForeignKey(uc => uc.UserId);

            builder.Entity<UserCompany>()
                .HasOne(uc => uc.Company)
                .WithMany(c => c.UserCompanies)
                .HasForeignKey(uc => uc.CompanyId);

            builder.Entity<Tenant>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Logo).HasMaxLength(200);
                entity.Property(e => e.Theme).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            builder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Logo).HasMaxLength(200);
                entity.Property(e => e.Theme).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

        }
    }
}
