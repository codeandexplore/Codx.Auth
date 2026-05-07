using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Auth.Data.Migrations.Users
{
    /// <inheritdoc />
    public partial class AddStatusColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "WorkspaceRoleDefinitions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedAt",
                table: "UserMemberships",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatusChangedBy",
                table: "UserMemberships",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedAt",
                table: "UserMembershipRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatusChangedBy",
                table: "UserMembershipRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "UserApplicationRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RevokedByUserId",
                table: "UserApplicationRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "UserApplicationRoles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EnterpriseApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EnterpriseApplicationRoles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EmailTemplates",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatusChangedBy",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMemberships_Status_User",
                table: "UserMemberships",
                columns: new[] { "UserId", "Status", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserApplicationRoles_Status_User",
                table: "UserApplicationRoles",
                columns: new[] { "UserId", "TenantId", "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Status",
                table: "Tenants",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_Active",
                table: "EmailTemplates",
                columns: new[] { "TemplateType", "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Status_Tenant",
                table: "Companies",
                columns: new[] { "Status", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserMemberships_Status_User",
                table: "UserMemberships");

            migrationBuilder.DropIndex(
                name: "IX_UserApplicationRoles_Status_User",
                table: "UserApplicationRoles");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Status",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_Active",
                table: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Status_Tenant",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "WorkspaceRoleDefinitions");

            migrationBuilder.DropColumn(
                name: "StatusChangedAt",
                table: "UserMemberships");

            migrationBuilder.DropColumn(
                name: "StatusChangedBy",
                table: "UserMemberships");

            migrationBuilder.DropColumn(
                name: "StatusChangedAt",
                table: "UserMembershipRoles");

            migrationBuilder.DropColumn(
                name: "StatusChangedBy",
                table: "UserMembershipRoles");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "UserApplicationRoles");

            migrationBuilder.DropColumn(
                name: "RevokedByUserId",
                table: "UserApplicationRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserApplicationRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EnterpriseApplications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EnterpriseApplicationRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StatusChangedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StatusChangedBy",
                table: "AspNetUsers");
        }
    }
}
