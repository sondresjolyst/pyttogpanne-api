using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pyttogpanne_api.Migrations
{
    /// <inheritdoc />
    public partial class AdvertisingDisclosure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Advertiser",
                table: "Recipes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdvertising",
                table: "Recipes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Advertiser",
                table: "GearItems",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdvertising",
                table: "GearItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Advertiser",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IsAdvertising",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Advertiser",
                table: "GearItems");

            migrationBuilder.DropColumn(
                name: "IsAdvertising",
                table: "GearItems");
        }
    }
}
