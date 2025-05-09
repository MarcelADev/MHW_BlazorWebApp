using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHWApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class ShiftGameExtensionToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArmorCategories_Categories_CategoryId1",
                table: "ArmorCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Armors_GameExtensions_GameExtensionId",
                table: "Armors");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryEntries_GameExtensions_GameExtensionId",
                table: "InventoryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryEntryCategories_Categories_CategoryId1",
                table: "InventoryEntryCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponCategories_Categories_CategoryId1",
                table: "WeaponCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_GameExtensions_GameExtensionId",
                table: "Weapons");

            migrationBuilder.DropIndex(
                name: "IX_Weapons_GameExtensionId",
                table: "Weapons");

            migrationBuilder.DropIndex(
                name: "IX_WeaponCategories_CategoryId1",
                table: "WeaponCategories");

            migrationBuilder.DropIndex(
                name: "IX_InventoryEntryCategories_CategoryId1",
                table: "InventoryEntryCategories");

            migrationBuilder.DropIndex(
                name: "IX_InventoryEntries_GameExtensionId",
                table: "InventoryEntries");

            migrationBuilder.DropIndex(
                name: "IX_Armors_GameExtensionId",
                table: "Armors");

            migrationBuilder.DropIndex(
                name: "IX_ArmorCategories_CategoryId1",
                table: "ArmorCategories");

            migrationBuilder.DropColumn(
                name: "GameExtensionId",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "WeaponCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "InventoryEntryCategories");

            migrationBuilder.DropColumn(
                name: "GameExtensionId",
                table: "InventoryEntries");

            migrationBuilder.DropColumn(
                name: "GameExtensionId",
                table: "Armors");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "ArmorCategories");

            migrationBuilder.AddColumn<int>(
                name: "GameExtensionId",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_GameExtensionId",
                table: "Categories",
                column: "GameExtensionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_GameExtensions_GameExtensionId",
                table: "Categories",
                column: "GameExtensionId",
                principalTable: "GameExtensions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_GameExtensions_GameExtensionId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_GameExtensionId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "GameExtensionId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "GameExtensionId",
                table: "Weapons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "WeaponCategories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "InventoryEntryCategories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameExtensionId",
                table: "InventoryEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GameExtensionId",
                table: "Armors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "ArmorCategories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_GameExtensionId",
                table: "Weapons",
                column: "GameExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponCategories_CategoryId1",
                table: "WeaponCategories",
                column: "CategoryId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntryCategories_CategoryId1",
                table: "InventoryEntryCategories",
                column: "CategoryId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntries_GameExtensionId",
                table: "InventoryEntries",
                column: "GameExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_Armors_GameExtensionId",
                table: "Armors",
                column: "GameExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_ArmorCategories_CategoryId1",
                table: "ArmorCategories",
                column: "CategoryId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ArmorCategories_Categories_CategoryId1",
                table: "ArmorCategories",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Armors_GameExtensions_GameExtensionId",
                table: "Armors",
                column: "GameExtensionId",
                principalTable: "GameExtensions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryEntries_GameExtensions_GameExtensionId",
                table: "InventoryEntries",
                column: "GameExtensionId",
                principalTable: "GameExtensions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryEntryCategories_Categories_CategoryId1",
                table: "InventoryEntryCategories",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponCategories_Categories_CategoryId1",
                table: "WeaponCategories",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_GameExtensions_GameExtensionId",
                table: "Weapons",
                column: "GameExtensionId",
                principalTable: "GameExtensions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
