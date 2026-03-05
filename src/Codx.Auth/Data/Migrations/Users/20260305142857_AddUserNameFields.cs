using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codx.Auth.Data.Migrations.Users
{
    /// <inheritdoc />
    public partial class AddUserNameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FamilyName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // Backfill GivenName from existing given_name claims
            migrationBuilder.Sql(@"
                UPDATE u
                SET u.GivenName = c.ClaimValue
                FROM AspNetUsers u
                INNER JOIN AspNetUserClaims c ON c.UserId = u.Id AND c.ClaimType = 'given_name'
                WHERE u.GivenName IS NULL AND c.ClaimValue IS NOT NULL AND c.ClaimValue <> '';
            ");

            // Backfill FamilyName from existing family_name claims
            migrationBuilder.Sql(@"
                UPDATE u
                SET u.FamilyName = c.ClaimValue
                FROM AspNetUsers u
                INNER JOIN AspNetUserClaims c ON c.UserId = u.Id AND c.ClaimType = 'family_name'
                WHERE u.FamilyName IS NULL AND c.ClaimValue IS NOT NULL AND c.ClaimValue <> '';
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FamilyName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "AspNetUsers");
        }
    }
}
