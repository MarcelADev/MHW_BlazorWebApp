using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHWApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class JoinInventoryEntryInventoryEntryCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryEntries_InventoryEntryCategories_InventoryCategoryId",
                table: "InventoryEntries");

            migrationBuilder.DropIndex(
                name: "IX_InventoryEntries_InventoryCategoryId",
                table: "InventoryEntries");

            migrationBuilder.DropColumn(
                name: "InventoryCategoryId",
                table: "InventoryEntries");

            migrationBuilder.CreateTable(
                name: "InventoryEntryInventoryEntryCategory",
                columns: table => new
                {
                    InventoryEntriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    InventoryEntryCategoriesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntryInventoryEntryCategory", x => new { x.InventoryEntriesId, x.InventoryEntryCategoriesId });
                    table.ForeignKey(
                        name: "FK_InventoryEntryInventoryEntryCategory_InventoryEntries_InventoryEntriesId",
                        column: x => x.InventoryEntriesId,
                        principalTable: "InventoryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryEntryInventoryEntryCategory_InventoryEntryCategories_InventoryEntryCategoriesId",
                        column: x => x.InventoryEntryCategoriesId,
                        principalTable: "InventoryEntryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntryInventoryEntryCategory_InventoryEntryCategoriesId",
                table: "InventoryEntryInventoryEntryCategory",
                column: "InventoryEntryCategoriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryEntryInventoryEntryCategory");

            migrationBuilder.AddColumn<int>(
                name: "InventoryCategoryId",
                table: "InventoryEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntries_InventoryCategoryId",
                table: "InventoryEntries",
                column: "InventoryCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryEntries_InventoryEntryCategories_InventoryCategoryId",
                table: "InventoryEntries",
                column: "InventoryCategoryId",
                principalTable: "InventoryEntryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
