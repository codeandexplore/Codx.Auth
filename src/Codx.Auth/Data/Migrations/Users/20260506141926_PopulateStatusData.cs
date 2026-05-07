using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Auth.Data.Migrations.Users
{
    /// <inheritdoc />
    public partial class PopulateStatusData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---- Tenants ----
            // IsDeleted=1 → Cancelled (terminal; treat as hard delete equivalent)
            migrationBuilder.Sql(@"
                UPDATE Tenants SET Status = 'Cancelled'
                WHERE IsDeleted = 1;");

            // IsDeleted=0, IsActive=0 → Suspended
            migrationBuilder.Sql(@"
                UPDATE Tenants SET Status = 'Suspended'
                WHERE IsDeleted = 0 AND IsActive = 0 AND Status IS NULL;");

            // IsDeleted=0, IsActive=1 → Active
            migrationBuilder.Sql(@"
                UPDATE Tenants SET Status = 'Active'
                WHERE IsDeleted = 0 AND IsActive = 1 AND Status IS NULL;");

            // ---- Companies ----
            migrationBuilder.Sql(@"
                UPDATE Companies SET Status = 'Cancelled'
                WHERE IsDeleted = 1;");

            migrationBuilder.Sql(@"
                UPDATE Companies SET Status = 'Suspended'
                WHERE IsDeleted = 0 AND IsActive = 0 AND Status IS NULL;");

            migrationBuilder.Sql(@"
                UPDATE Companies SET Status = 'Active'
                WHERE IsDeleted = 0 AND IsActive = 1 AND Status IS NULL;");

            // ---- UserMemberships ----
            // 'Inactive' was used for removal; map to the canonical 'Removed' status
            migrationBuilder.Sql(@"
                UPDATE UserMemberships SET Status = 'Removed'
                WHERE Status = 'Inactive';");
            // All other existing statuses ('Active', 'Suspended') are already correct — no action needed.

            // ---- UserMembershipRoles ----
            migrationBuilder.Sql(@"
                UPDATE UserMembershipRoles SET Status = 'Removed'
                WHERE Status = 'Inactive';");

            // ---- EnterpriseApplications ----
            migrationBuilder.Sql(@"
                UPDATE EnterpriseApplications SET Status = 'Active'
                WHERE IsActive = 1;");

            migrationBuilder.Sql(@"
                UPDATE EnterpriseApplications SET Status = 'Inactive'
                WHERE IsActive = 0;");

            // ---- EnterpriseApplicationRoles ----
            migrationBuilder.Sql(@"
                UPDATE EnterpriseApplicationRoles SET Status = 'Active'
                WHERE IsActive = 1;");

            migrationBuilder.Sql(@"
                UPDATE EnterpriseApplicationRoles SET Status = 'Inactive'
                WHERE IsActive = 0;");

            // ---- WorkspaceRoleDefinitions ----
            migrationBuilder.Sql(@"
                UPDATE WorkspaceRoleDefinitions SET Status = 'Active'
                WHERE IsActive = 1;");

            migrationBuilder.Sql(@"
                UPDATE WorkspaceRoleDefinitions SET Status = 'Inactive'
                WHERE IsActive = 0;");

            // ---- UserApplicationRoles ----
            // No prior lifecycle tracking; all existing rows are live assignments → Active
            migrationBuilder.Sql(@"
                UPDATE UserApplicationRoles SET Status = 'Active'
                WHERE Status IS NULL;");

            // ---- AspNetUsers ----
            // No prior lifecycle tracking; all existing users are active
            migrationBuilder.Sql(@"
                UPDATE AspNetUsers SET Status = 'Active'
                WHERE Status IS NULL;");

            // ---- EmailTemplates ----
            // No prior lifecycle tracking; all existing templates are active
            migrationBuilder.Sql(@"
                UPDATE EmailTemplates SET Status = 'Active'
                WHERE Status IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse: clear all Status values populated by this migration
            // (columns revert to NULL as they were before Migration A data was populated)
            migrationBuilder.Sql("UPDATE Tenants SET Status = NULL;");
            migrationBuilder.Sql("UPDATE Companies SET Status = NULL;");
            migrationBuilder.Sql("UPDATE UserApplicationRoles SET Status = NULL;");
            migrationBuilder.Sql("UPDATE AspNetUsers SET Status = NULL;");
            migrationBuilder.Sql("UPDATE EmailTemplates SET Status = NULL;");
            migrationBuilder.Sql("UPDATE EnterpriseApplications SET Status = NULL;");
            migrationBuilder.Sql("UPDATE EnterpriseApplicationRoles SET Status = NULL;");
            migrationBuilder.Sql("UPDATE WorkspaceRoleDefinitions SET Status = NULL;");

            // Restore UserMemberships/UserMembershipRoles: 'Removed' → 'Inactive' (original value)
            migrationBuilder.Sql("UPDATE UserMemberships SET Status = 'Inactive' WHERE Status = 'Removed';");
            migrationBuilder.Sql("UPDATE UserMembershipRoles SET Status = 'Inactive' WHERE Status = 'Removed';");
        }
    }
}
