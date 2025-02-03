using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHWApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class MigrateCategoryAndGameExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Armors_GameExtension_GameExtensionId",
                table: "Armors");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryEntries_GameExtension_GameExtensionId",
                table: "InventoryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_GameExtension_GameExtensionId",
                table: "Weapons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameExtension",
                table: "GameExtension");

            migrationBuilder.RenameTable(
                name: "GameExtension",
                newName: "GameExtensions");

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
                name: "CategoryId1",
                table: "ArmorCategories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameExtensions",
                table: "GameExtensions",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "IX_WeaponCategories_CategoryId1",
                table: "WeaponCategories");

            migrationBuilder.DropIndex(
                name: "IX_InventoryEntryCategories_CategoryId1",
                table: "InventoryEntryCategories");

            migrationBuilder.DropIndex(
                name: "IX_ArmorCategories_CategoryId1",
                table: "ArmorCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameExtensions",
                table: "GameExtensions");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "WeaponCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "InventoryEntryCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "ArmorCategories");

            migrationBuilder.RenameTable(
                name: "GameExtensions",
                newName: "GameExtension");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameExtension",
                table: "GameExtension",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Armors_GameExtension_GameExtensionId",
                table: "Armors",
                column: "GameExtensionId",
                principalTable: "GameExtension",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryEntries_GameExtension_GameExtensionId",
                table: "InventoryEntries",
                column: "GameExtensionId",
                principalTable: "GameExtension",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_GameExtension_GameExtensionId",
                table: "Weapons",
                column: "GameExtensionId",
                principalTable: "GameExtension",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
