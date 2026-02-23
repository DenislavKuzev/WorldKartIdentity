using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class FixFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrackAnnotations_TrackId1",
                table: "TrackAnnotations");

            migrationBuilder.AddColumn<int>(
                name: "TrackTrajectoryId",
                table: "TrackAnnotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_TrackId1",
                table: "TrackAnnotations",
                column: "TrackId1");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_TrackTrajectoryId",
                table: "TrackAnnotations",
                column: "TrackTrajectoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackAnnotations_TrackTrajectory_TrackTrajectoryId",
                table: "TrackAnnotations",
                column: "TrackTrajectoryId",
                principalTable: "TrackTrajectory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackAnnotations_TrackTrajectory_TrackTrajectoryId",
                table: "TrackAnnotations");

            migrationBuilder.DropIndex(
                name: "IX_TrackAnnotations_TrackId1",
                table: "TrackAnnotations");

            migrationBuilder.DropIndex(
                name: "IX_TrackAnnotations_TrackTrajectoryId",
                table: "TrackAnnotations");

            migrationBuilder.DropColumn(
                name: "TrackTrajectoryId",
                table: "TrackAnnotations");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_TrackId1",
                table: "TrackAnnotations",
                column: "TrackId1",
                unique: true,
                filter: "[TrackId1] IS NOT NULL");
        }
    }
}
