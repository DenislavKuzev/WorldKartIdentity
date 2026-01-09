using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapsLink",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelNumber",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Worktime",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "GoogleMapsLink",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "TelNumber",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "Worktime",
                table: "Tracks");
        }
    }
}
