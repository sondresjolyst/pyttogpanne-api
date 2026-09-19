using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pyttogpanne_api.Migrations
{
    /// <inheritdoc />
    public partial class RecipePhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_ContentImages_CoverImageId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_CoverImageId",
                table: "Recipes");

            migrationBuilder.CreateTable(
                name: "RecipeImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipeId = table.Column<int>(type: "integer", nullable: false),
                    ContentImageId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Caption = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeImages_ContentImages_ContentImageId",
                        column: x => x.ContentImageId,
                        principalTable: "ContentImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeImages_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeImages_ContentImageId",
                table: "RecipeImages",
                column: "ContentImageId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeImages_RecipeId_ContentImageId",
                table: "RecipeImages",
                columns: new[] { "RecipeId", "ContentImageId" },
                unique: true);

            // Carry each existing cover into the gallery as its first photo, before the column
            // holding it goes away.
            migrationBuilder.Sql("""
                INSERT INTO "RecipeImages" ("RecipeId", "ContentImageId", "SortOrder")
                SELECT "Id", "CoverImageId", 0 FROM "Recipes" WHERE "CoverImageId" IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "CoverImageId",
                table: "Recipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeImages");

            migrationBuilder.AddColumn<string>(
                name: "CoverImageId",
                table: "Recipes",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CoverImageId",
                table: "Recipes",
                column: "CoverImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_ContentImages_CoverImageId",
                table: "Recipes",
                column: "CoverImageId",
                principalTable: "ContentImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
