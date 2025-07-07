using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHWApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class GameExtensionRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_GameExtensionId",
                table: "Weapons",
                column: "GameExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntries_GameExtensionId",
                table: "InventoryEntries",
                column: "GameExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_Armors_GameExtensionId",
                table: "Armors",
                column: "GameExtensionId");

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
                name: "FK_Armors_GameExtensions_GameExtensionId",
                table: "Armors");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryEntries_GameExtensions_GameExtensionId",
                table: "InventoryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_GameExtensions_GameExtensionId",
                table: "Weapons");

            migrationBuilder.DropIndex(
                name: "IX_Weapons_GameExtensionId",
                table: "Weapons");

            migrationBuilder.DropIndex(
                name: "IX_InventoryEntries_GameExtensionId",
                table: "InventoryEntries");

            migrationBuilder.DropIndex(
                name: "IX_Armors_GameExtensionId",
                table: "Armors");

            migrationBuilder.DropColumn(
                name: "GameExtensionId",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "GameExtensionId",
                table: "InventoryEntries");

            migrationBuilder.DropColumn(
                name: "GameExtensionId",
                table: "Armors");

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
    }
}
