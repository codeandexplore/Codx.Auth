using Codx.Auth.Infrastructure.Lifecycle;

namespace Codx.Auth.Unit.Test.Lifecycle
{
    /// <summary>
    /// Unit tests for <see cref="LifecycleTransitionGuard"/>.
    /// Covers all entity categories and all valid/invalid transitions.
    /// </summary>
    public class LifecycleTransitionGuardTests
    {
        private readonly LifecycleTransitionGuard _guard = new();

        // ─── Tenant ────────────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Suspended")]
        [InlineData("Active", "Cancelled")]
        [InlineData("Suspended", "Active")]
        [InlineData("Suspended", "Cancelled")]
        public void Tenant_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("Tenant", from, to);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("Cancelled", "Active")]
        [InlineData("Cancelled", "Suspended")]
        [InlineData("Active", "Active")]
        [InlineData("Active", "Unknown")]
        public void Tenant_ForbiddenTransitions_ReturnFail(string from, string to)
        {
            var result = _guard.Validate("Tenant", from, to);
            Assert.False(result.IsValid);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public void Tenant_Cancelled_IsTerminal()
        {
            var result = _guard.Validate("Tenant", LifecycleStatus.Tenant.Cancelled, LifecycleStatus.Tenant.Active);
            Assert.False(result.IsValid);
        }

        // ─── Company ───────────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Suspended")]
        [InlineData("Active", "Cancelled")]
        [InlineData("Suspended", "Active")]
        [InlineData("Suspended", "Cancelled")]
        public void Company_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("Company", from, to);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("Cancelled", "Active")]
        [InlineData("Cancelled", "Suspended")]
        public void Company_Cancelled_IsTerminal(string from, string to)
        {
            var result = _guard.Validate("Company", from, to);
            Assert.False(result.IsValid);
        }

        // ─── Membership ────────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Suspended")]
        [InlineData("Active", "Removed")]
        [InlineData("Suspended", "Active")]
        [InlineData("Suspended", "Removed")]
        public void Membership_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("Membership", from, to);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("Removed", "Active")]
        [InlineData("Removed", "Suspended")]
        public void Membership_Removed_IsTerminal(string from, string to)
        {
            var result = _guard.Validate("Membership", from, to);
            Assert.False(result.IsValid);
        }

        // ─── MembershipRole ────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Suspended")]
        [InlineData("Active", "Removed")]
        [InlineData("Suspended", "Active")]
        [InlineData("Suspended", "Removed")]
        public void MembershipRole_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("MembershipRole", from, to);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("Removed", "Active")]
        [InlineData("Removed", "Suspended")]
        public void MembershipRole_Removed_IsTerminal(string from, string to)
        {
            var result = _guard.Validate("MembershipRole", from, to);
            Assert.False(result.IsValid);
        }

        // ─── Application ───────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Inactive")]
        [InlineData("Inactive", "Active")]
        public void Application_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("Application", from, to);
            Assert.True(result.IsValid);
        }

        // ─── AppRole ───────────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Inactive")]
        [InlineData("Inactive", "Active")]
        public void AppRole_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("AppRole", from, to);
            Assert.True(result.IsValid);
        }

        // ─── RoleDefinition ────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Inactive")]
        [InlineData("Inactive", "Active")]
        public void RoleDefinition_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("RoleDefinition", from, to);
            Assert.True(result.IsValid);
        }

        // ─── RoleAssignment ────────────────────────────────────────────────────

        [Fact]
        public void RoleAssignment_Active_CanBeRevoked()
        {
            var result = _guard.Validate("RoleAssignment", LifecycleStatus.RoleAssignment.Active, LifecycleStatus.RoleAssignment.Revoked);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void RoleAssignment_Revoked_IsTerminal()
        {
            var result = _guard.Validate("RoleAssignment", LifecycleStatus.RoleAssignment.Revoked, LifecycleStatus.RoleAssignment.Active);
            Assert.False(result.IsValid);
        }

        // ─── User ──────────────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Suspended")]
        [InlineData("Active", "Deactivated")]
        [InlineData("Suspended", "Active")]
        [InlineData("Suspended", "Deactivated")]
        public void User_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("User", from, to);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("Deactivated", "Active")]
        [InlineData("Deactivated", "Suspended")]
        public void User_Deactivated_IsTerminal(string from, string to)
        {
            var result = _guard.Validate("User", from, to);
            Assert.False(result.IsValid);
        }

        // ─── EmailTemplate ─────────────────────────────────────────────────────

        [Theory]
        [InlineData("Active", "Archived")]
        [InlineData("Archived", "Active")]
        public void EmailTemplate_AllowedTransitions_ReturnOk(string from, string to)
        {
            var result = _guard.Validate("EmailTemplate", from, to);
            Assert.True(result.IsValid);
        }

        // ─── Unknown category / status ─────────────────────────────────────────

        [Fact]
        public void UnknownCategory_ReturnsFail_WithMessage()
        {
            var result = _guard.Validate("Widget", "Active", "Inactive");
            Assert.False(result.IsValid);
            Assert.Contains("Widget", result.Error);
        }

        [Fact]
        public void KnownCategory_UnknownCurrentStatus_ReturnsFail()
        {
            var result = _guard.Validate("Tenant", "Draft", "Active");
            Assert.False(result.IsValid);
            Assert.Contains("Draft", result.Error);
        }

        [Fact]
        public void ValidTransition_ReturnsNullError()
        {
            var result = _guard.Validate("Tenant", LifecycleStatus.Tenant.Active, LifecycleStatus.Tenant.Suspended);
            Assert.True(result.IsValid);
            Assert.Null(result.Error);
        }
    }
}
