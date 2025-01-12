using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHWApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameExtension",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameExtension", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeaponTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArmorCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArmorCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArmorCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryEntryCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntryCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryEntryCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participators_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participators_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participators_WeaponTypes_WeaponTypeId",
                        column: x => x.WeaponTypeId,
                        principalTable: "WeaponTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Armors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArmorCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameExtensionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Armors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Armors_ArmorCategories_ArmorCategoryId",
                        column: x => x.ArmorCategoryId,
                        principalTable: "ArmorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Armors_GameExtension_GameExtensionId",
                        column: x => x.GameExtensionId,
                        principalTable: "GameExtension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InventoryCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameExtensionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryEntries_GameExtension_GameExtensionId",
                        column: x => x.GameExtensionId,
                        principalTable: "GameExtension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryEntries_InventoryEntryCategories_InventoryCategoryId",
                        column: x => x.InventoryCategoryId,
                        principalTable: "InventoryEntryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WeaponCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameExtensionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PredecessorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    WeaponTier = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weapons_GameExtension_GameExtensionId",
                        column: x => x.GameExtensionId,
                        principalTable: "GameExtension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weapons_WeaponCategories_WeaponCategoryId",
                        column: x => x.WeaponCategoryId,
                        principalTable: "WeaponCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weapons_WeaponTypes_WeaponTypeId",
                        column: x => x.WeaponTypeId,
                        principalTable: "WeaponTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weapons_Weapons_PredecessorId",
                        column: x => x.PredecessorId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipatorArmor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParticipatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArmorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipatorArmor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipatorArmor_Armors_ArmorId",
                        column: x => x.ArmorId,
                        principalTable: "Armors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipatorArmor_Participators_ParticipatorId",
                        column: x => x.ParticipatorId,
                        principalTable: "Participators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArmorCraftingIngredient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArmorId = table.Column<int>(type: "INTEGER", nullable: false),
                    InventoryEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArmorCraftingIngredient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArmorCraftingIngredient_Armors_ArmorId",
                        column: x => x.ArmorId,
                        principalTable: "Armors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArmorCraftingIngredient_InventoryEntries_InventoryEntryId",
                        column: x => x.InventoryEntryId,
                        principalTable: "InventoryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipatorInventoryEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParticipatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    InventoryEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipatorInventoryEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipatorInventoryEntry_InventoryEntries_InventoryEntryId",
                        column: x => x.InventoryEntryId,
                        principalTable: "InventoryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipatorInventoryEntry_Participators_ParticipatorId",
                        column: x => x.ParticipatorId,
                        principalTable: "Participators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipatorWeapon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParticipatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipatorWeapon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipatorWeapon_Participators_ParticipatorId",
                        column: x => x.ParticipatorId,
                        principalTable: "Participators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipatorWeapon_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponCraftingIngredient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: false),
                    InventoryEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponCraftingIngredient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponCraftingIngredient_InventoryEntries_InventoryEntryId",
                        column: x => x.InventoryEntryId,
                        principalTable: "InventoryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponCraftingIngredient_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArmorCategories_CategoryId",
                table: "ArmorCategories",
                column: "CategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArmorCraftingIngredient_ArmorId",
                table: "ArmorCraftingIngredient",
                column: "ArmorId");

            migrationBuilder.CreateIndex(
                name: "IX_ArmorCraftingIngredient_InventoryEntryId",
                table: "ArmorCraftingIngredient",
                column: "InventoryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Armors_ArmorCategoryId",
                table: "Armors",
                column: "ArmorCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Armors_GameExtensionId",
                table: "Armors",
                column: "GameExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntries_GameExtensionId",
                table: "InventoryEntries",
                column: "GameExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntries_InventoryCategoryId",
                table: "InventoryEntries",
                column: "InventoryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntryCategories_CategoryId",
                table: "InventoryEntryCategories",
                column: "CategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipatorArmor_ArmorId",
                table: "ParticipatorArmor",
                column: "ArmorId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipatorArmor_ParticipatorId",
                table: "ParticipatorArmor",
                column: "ParticipatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipatorInventoryEntry_InventoryEntryId",
                table: "ParticipatorInventoryEntry",
                column: "InventoryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipatorInventoryEntry_ParticipatorId",
                table: "ParticipatorInventoryEntry",
                column: "ParticipatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Participators_PlayerId",
                table: "Participators",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Participators_SessionId",
                table: "Participators",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Participators_WeaponTypeId",
                table: "Participators",
                column: "WeaponTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipatorWeapon_ParticipatorId",
                table: "ParticipatorWeapon",
                column: "ParticipatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipatorWeapon_WeaponId",
                table: "ParticipatorWeapon",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponCategories_CategoryId",
                table: "WeaponCategories",
                column: "CategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeaponCraftingIngredient_InventoryEntryId",
                table: "WeaponCraftingIngredient",
                column: "InventoryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponCraftingIngredient_WeaponId",
                table: "WeaponCraftingIngredient",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_GameExtensionId",
                table: "Weapons",
                column: "GameExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_PredecessorId",
                table: "Weapons",
                column: "PredecessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_WeaponCategoryId",
                table: "Weapons",
                column: "WeaponCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_WeaponTypeId",
                table: "Weapons",
                column: "WeaponTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArmorCraftingIngredient");

            migrationBuilder.DropTable(
                name: "ParticipatorArmor");

            migrationBuilder.DropTable(
                name: "ParticipatorInventoryEntry");

            migrationBuilder.DropTable(
                name: "ParticipatorWeapon");

            migrationBuilder.DropTable(
                name: "WeaponCraftingIngredient");

            migrationBuilder.DropTable(
                name: "Armors");

            migrationBuilder.DropTable(
                name: "Participators");

            migrationBuilder.DropTable(
                name: "InventoryEntries");

            migrationBuilder.DropTable(
                name: "Weapons");

            migrationBuilder.DropTable(
                name: "ArmorCategories");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "InventoryEntryCategories");

            migrationBuilder.DropTable(
                name: "GameExtension");

            migrationBuilder.DropTable(
                name: "WeaponCategories");

            migrationBuilder.DropTable(
                name: "WeaponTypes");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
