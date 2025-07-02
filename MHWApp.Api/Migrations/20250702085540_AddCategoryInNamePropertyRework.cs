using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHWApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryInNamePropertyRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryInName",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "CategoryInName",
                table: "InventoryEntries");

            migrationBuilder.DropColumn(
                name: "CategoryInName",
                table: "Armors");

            migrationBuilder.AddColumn<int>(
                name: "NamePrefixId",
                table: "InventoryEntries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NamePrefixId",
                table: "Armors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntries_NamePrefixId",
                table: "InventoryEntries",
                column: "NamePrefixId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryEntries_InventoryEntryCategories_NamePrefixId",
                table: "InventoryEntries",
                column: "NamePrefixId",
                principalTable: "InventoryEntryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Armors_ArmorCategories_NamePrefixId",
                table: "Armors");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryEntries_InventoryEntryCategories_NamePrefixId",
                table: "InventoryEntries");

            migrationBuilder.DropIndex(
                name: "IX_InventoryEntries_NamePrefixId",
                table: "InventoryEntries");

            migrationBuilder.DropIndex(
                name: "IX_Armors_NamePrefixId",
                table: "Armors");

            migrationBuilder.DropColumn(
                name: "NamePrefixId",
                table: "InventoryEntries");

            migrationBuilder.DropColumn(
                name: "NamePrefixId",
                table: "Armors");

            migrationBuilder.AddColumn<bool>(
                name: "CategoryInName",
                table: "Weapons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CategoryInName",
                table: "InventoryEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CategoryInName",
                table: "Armors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
