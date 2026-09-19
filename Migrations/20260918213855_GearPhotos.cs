using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pyttogpanne_api.Migrations
{
    /// <inheritdoc />
    public partial class GearPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GearItems_ContentImages_ContentImageId",
                table: "GearItems");

            migrationBuilder.DropIndex(
                name: "IX_GearItems_ContentImageId",
                table: "GearItems");

            migrationBuilder.CreateTable(
                name: "GearImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GearItemId = table.Column<int>(type: "integer", nullable: false),
                    ContentImageId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Caption = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GearImages_ContentImages_ContentImageId",
                        column: x => x.ContentImageId,
                        principalTable: "ContentImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GearImages_GearItems_GearItemId",
                        column: x => x.GearItemId,
                        principalTable: "GearItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GearImages_ContentImageId",
                table: "GearImages",
                column: "ContentImageId");

            migrationBuilder.CreateIndex(
                name: "IX_GearImages_GearItemId_ContentImageId",
                table: "GearImages",
                columns: new[] { "GearItemId", "ContentImageId" },
                unique: true);

            // Carry each existing photo into the gallery as its first photo, before the column
            // holding it goes away.
            migrationBuilder.Sql("""
                INSERT INTO "GearImages" ("GearItemId", "ContentImageId", "SortOrder")
                SELECT "Id", "ContentImageId", 0 FROM "GearItems" WHERE "ContentImageId" IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "ContentImageId",
                table: "GearItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GearImages");

            migrationBuilder.AddColumn<string>(
                name: "ContentImageId",
                table: "GearItems",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GearItems_ContentImageId",
                table: "GearItems",
                column: "ContentImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_GearItems_ContentImages_ContentImageId",
                table: "GearItems",
                column: "ContentImageId",
                principalTable: "ContentImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
