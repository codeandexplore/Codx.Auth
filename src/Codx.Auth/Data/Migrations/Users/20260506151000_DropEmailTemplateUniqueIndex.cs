using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Auth.Data.Migrations.Users
{
    /// <inheritdoc />
    public partial class DropEmailTemplateUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_TenantId_TemplateType",
                table: "EmailTemplates");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_TenantId_TemplateType",
                table: "EmailTemplates",
                columns: new[] { "TenantId", "TemplateType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_TenantId_TemplateType",
                table: "EmailTemplates");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_TenantId_TemplateType",
                table: "EmailTemplates",
                columns: new[] { "TenantId", "TemplateType" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }
    }
}
