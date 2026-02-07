using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Data.Entities.Enterprise;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

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
        public DbSet<TenantManager> TenantManagers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }
        public DbSet<TwoFactorCode> TwoFactorCodes { get; set; }

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

            builder.Entity<TenantManager>()
                .HasKey(tm => new { tm.TenantId, tm.UserId });

            builder.Entity<TenantManager>()
                .HasOne(tm => tm.Manager)
                .WithMany(u => u.TenantManagers)
                .HasForeignKey(tm => tm.UserId);

            builder.Entity<TenantManager>()
                .HasOne(tm => tm.Tenant)
                .WithMany(t => t.TenantManagers)
                .HasForeignKey(tm => tm.TenantId);

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

            // Two-Factor Authentication Code entity configuration
            builder.Entity<TwoFactorCode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.IsUsed).HasDefaultValue(false);
                
                // Index for faster lookups
                entity.HasIndex(e => new { e.UserId, e.Code, e.ExpiresAt });
                
                // Foreign key relationship
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
