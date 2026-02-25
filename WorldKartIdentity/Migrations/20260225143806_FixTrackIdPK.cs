using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class FixTrackIdPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackAnnotations_Tracks_TrackId1",
                table: "TrackAnnotations");

            migrationBuilder.DropIndex(
                name: "IX_TrackAnnotations_TrackId",
                table: "TrackAnnotations");

            migrationBuilder.DropIndex(
                name: "IX_TrackAnnotations_TrackId1",
                table: "TrackAnnotations");

            migrationBuilder.DropColumn(
                name: "TrackId1",
                table: "TrackAnnotations");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_TrackId",
                table: "TrackAnnotations",
                column: "TrackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrackAnnotations_TrackId",
                table: "TrackAnnotations");

            migrationBuilder.AddColumn<int>(
                name: "TrackId1",
                table: "TrackAnnotations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_TrackId",
                table: "TrackAnnotations",
                column: "TrackId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_TrackId1",
                table: "TrackAnnotations",
                column: "TrackId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackAnnotations_Tracks_TrackId1",
                table: "TrackAnnotations",
                column: "TrackId1",
                principalTable: "Tracks",
                principalColumn: "Id");
        }
    }
}
