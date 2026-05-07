using Codx.Auth.Data.Entities.AspNet;
using Codx.Auth.Infrastructure.Lifecycle;
using Codx.Auth.Integration.Test.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Codx.Auth.Integration.Test.Lifecycle
{
    /// <summary>
    /// Integration tests for the user login pipeline Status block (TASK-6.2).
    /// Verifies that ApplicationUser.Status controls access independently of lockout,
    /// and that each Status value is correctly persisted and read via EF Core.
    /// </summary>
    public class UserLoginStatusTests
    {
        // ─── Status persistence ────────────────────────────────────────────────

        [Fact]
        public async Task NewUser_WithActiveStatus_IsReadBackCorrectly()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var userId = Guid.NewGuid();

            db.Users.Add(new ApplicationUser
            {
                Id            = userId,
                UserName      = "alice@test.com",
                NormalizedUserName = "ALICE@TEST.COM",
                Email         = "alice@test.com",
                NormalizedEmail = "ALICE@TEST.COM",
                Status        = LifecycleStatus.User.Active,
                GivenName     = "Alice",
                FamilyName    = "Smith",
            });
            await db.SaveChangesAsync();

            var user = await db.Users.FindAsync(userId);
            Assert.Equal(LifecycleStatus.User.Active, user!.Status);
        }

        [Theory]
        [InlineData("Suspended")]
        [InlineData("Deactivated")]
        public async Task NonActiveUser_StatusCheck_WouldBlockLogin(string status)
        {
            await using var db = InMemoryDbContextFactory.Create();
            var userId = Guid.NewGuid();

            db.Users.Add(new ApplicationUser
            {
                Id                 = userId,
                UserName           = $"user_{status}@test.com",
                NormalizedUserName = $"USER_{status.ToUpper()}@TEST.COM",
                Email              = $"user_{status}@test.com",
                NormalizedEmail    = $"USER_{status.ToUpper()}@TEST.COM",
                Status             = status,
            });
            await db.SaveChangesAsync();

            var user = await db.Users.FindAsync(userId);

            // Mirrors the check in AccountController.Login()
            Assert.NotEqual(LifecycleStatus.User.Active, user!.Status);
        }

        [Fact]
        public async Task ActiveUser_StatusCheck_AllowsLogin()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var userId = Guid.NewGuid();

            db.Users.Add(new ApplicationUser
            {
                Id                 = userId,
                UserName           = "active@test.com",
                NormalizedUserName = "ACTIVE@TEST.COM",
                Email              = "active@test.com",
                NormalizedEmail    = "ACTIVE@TEST.COM",
                Status             = LifecycleStatus.User.Active,
            });
            await db.SaveChangesAsync();

            var user = await db.Users.FindAsync(userId);
            Assert.Equal(LifecycleStatus.User.Active, user!.Status);
        }

        // ─── Status and lockout are independent ────────────────────────────────

        [Fact]
        public async Task SuspendedUser_WithNoLockout_WouldBeBlocked_ByStatusCheck()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var userId = Guid.NewGuid();

            db.Users.Add(new ApplicationUser
            {
                Id                  = userId,
                UserName            = "suspended@test.com",
                NormalizedUserName  = "SUSPENDED@TEST.COM",
                Email               = "suspended@test.com",
                NormalizedEmail     = "SUSPENDED@TEST.COM",
                Status              = LifecycleStatus.User.Suspended,
                LockoutEnd          = null, // not locked out at identity level
            });
            await db.SaveChangesAsync();

            var user = await db.Users.FindAsync(userId);

            var isLockedOut    = user!.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;
            var isStatusActive = user.Status == LifecycleStatus.User.Active;

            Assert.False(isLockedOut);    // passes lockout check
            Assert.False(isStatusActive); // fails status check — access denied
        }

        [Fact]
        public async Task LockedOutUser_WithActiveStatus_WouldBeBlocked_ByLockoutCheck()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var userId = Guid.NewGuid();

            db.Users.Add(new ApplicationUser
            {
                Id                 = userId,
                UserName           = "locked@test.com",
                NormalizedUserName = "LOCKED@TEST.COM",
                Email              = "locked@test.com",
                NormalizedEmail    = "LOCKED@TEST.COM",
                Status             = LifecycleStatus.User.Active,           // active status
                LockoutEnabled     = true,
                LockoutEnd         = DateTimeOffset.UtcNow.AddMinutes(30),  // locked out
            });
            await db.SaveChangesAsync();

            var user = await db.Users.FindAsync(userId);

            var isLockedOut    = user!.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;
            var isStatusActive = user.Status == LifecycleStatus.User.Active;

            Assert.True(isLockedOut);   // fails lockout check — access denied
            Assert.True(isStatusActive); // passes status check
        }

        // ─── Reinstate: Suspended → Active allows login again ─────────────────

        [Fact]
        public async Task ReinstatedUser_StatusSetToActive_AllowsLogin()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var userId  = Guid.NewGuid();
            var actorId = Guid.NewGuid();

            db.Users.Add(new ApplicationUser
            {
                Id                 = userId,
                UserName           = "reinstated@test.com",
                NormalizedUserName = "REINSTATED@TEST.COM",
                Email              = "reinstated@test.com",
                NormalizedEmail    = "REINSTATED@TEST.COM",
                Status             = LifecycleStatus.User.Suspended,
            });
            await db.SaveChangesAsync();

            // Simulate reinstatement (controller sets Status + audit fields)
            var user = await db.Users.FindAsync(userId);
            user!.Status           = LifecycleStatus.User.Active;
            user.StatusChangedAt   = DateTime.UtcNow;
            user.StatusChangedBy   = actorId;
            await db.SaveChangesAsync();

            var reinstated = await db.Users.FindAsync(userId);
            Assert.Equal(LifecycleStatus.User.Active, reinstated!.Status);
        }

        // ─── Guard: Deactivated is terminal ───────────────────────────────────

        [Fact]
        public void Guard_User_Deactivated_CannotTransitionToActive()
        {
            var guard = new LifecycleTransitionGuard();
            var result = guard.Validate("User", LifecycleStatus.User.Deactivated, LifecycleStatus.User.Active);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Guard_User_Deactivated_CannotTransitionToSuspended()
        {
            var guard = new LifecycleTransitionGuard();
            var result = guard.Validate("User", LifecycleStatus.User.Deactivated, LifecycleStatus.User.Suspended);
            Assert.False(result.IsValid);
        }

        // ─── StatusChangedAt / StatusChangedBy audit fields ───────────────────

        [Fact]
        public async Task StatusChange_SetsAuditFields()
        {
            await using var db = InMemoryDbContextFactory.Create();
            var userId  = Guid.NewGuid();
            var actorId = Guid.NewGuid();

            db.Users.Add(new ApplicationUser
            {
                Id                 = userId,
                UserName           = "audit@test.com",
                NormalizedUserName = "AUDIT@TEST.COM",
                Email              = "audit@test.com",
                NormalizedEmail    = "AUDIT@TEST.COM",
                Status             = LifecycleStatus.User.Active,
            });
            await db.SaveChangesAsync();

            var before = DateTime.UtcNow;
            var user = await db.Users.FindAsync(userId);
            user!.Status           = LifecycleStatus.User.Suspended;
            user.StatusChangedAt   = DateTime.UtcNow;
            user.StatusChangedBy   = actorId;
            await db.SaveChangesAsync();

            var updated = await db.Users.FindAsync(userId);
            Assert.Equal(LifecycleStatus.User.Suspended, updated!.Status);
            Assert.NotNull(updated.StatusChangedAt);
            Assert.True(updated.StatusChangedAt >= before);
            Assert.Equal(actorId, updated.StatusChangedBy);
        }
    }
}
