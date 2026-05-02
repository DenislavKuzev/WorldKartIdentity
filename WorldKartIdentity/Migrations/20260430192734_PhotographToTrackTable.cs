using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class PhotographToTrackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Tracks",
                newName: "RoutePicture");

            migrationBuilder.AddColumn<string>(
                name: "Photograph",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photograph",
                table: "Tracks");

            migrationBuilder.RenameColumn(
                name: "RoutePicture",
                table: "Tracks",
                newName: "Picture");
        }
    }
}
