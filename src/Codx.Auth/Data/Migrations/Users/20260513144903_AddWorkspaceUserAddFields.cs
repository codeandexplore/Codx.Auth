using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Auth.Data.Migrations.Users
{
    /// <inheritdoc />
    public partial class AddWorkspaceUserAddFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationId",
                table: "Invitations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationRoles",
                table: "Invitations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MembershipRole",
                table: "Invitations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RedirectUri",
                table: "Invitations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // Filtered index for duplicate-pending-invitation check (SQL Server partial index)
            migrationBuilder.Sql(@"
                CREATE INDEX IX_Invitations_Email_TenantId_CompanyId_Pending
                ON Invitations (Email, TenantId, CompanyId)
                WHERE Status = 'Pending'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "ApplicationRoles",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "MembershipRole",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "RedirectUri",
                table: "Invitations");

            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Invitations_Email_TenantId_CompanyId_Pending ON Invitations");
        }
    }
}
