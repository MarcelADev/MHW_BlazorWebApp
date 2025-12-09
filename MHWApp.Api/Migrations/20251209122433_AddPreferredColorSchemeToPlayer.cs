using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHWApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferredColorSchemeToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreferredColorScheme",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredColorScheme",
                table: "AspNetUsers");
        }
    }
}
