using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHWApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryInNamePropertyRework2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Armors_ArmorCategories_NamePrefixId",
                table: "Armors");

            migrationBuilder.DropIndex(
                name: "IX_Armors_NamePrefixId",
                table: "Armors");

            migrationBuilder.DropColumn(
                name: "NamePrefixId",
                table: "Armors");

            migrationBuilder.AddColumn<bool>(
                name: "NamePrefix",
                table: "Armors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NamePrefix",
                table: "Armors");

            migrationBuilder.AddColumn<int>(
                name: "NamePrefixId",
                table: "Armors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Armors_NamePrefixId",
                table: "Armors",
                column: "NamePrefixId");

            migrationBuilder.AddForeignKey(
                name: "FK_Armors_ArmorCategories_NamePrefixId",
                table: "Armors",
                column: "NamePrefixId",
                principalTable: "ArmorCategories",
                principalColumn: "Id");
        }
    }
}
