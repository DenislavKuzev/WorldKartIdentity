using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class EditTrajectoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TrackTrajectory",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrackTrajectory_UserId",
                table: "TrackTrajectory",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTrajectory_Users_UserId",
                table: "TrackTrajectory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackTrajectory_Users_UserId",
                table: "TrackTrajectory");

            migrationBuilder.DropIndex(
                name: "IX_TrackTrajectory_UserId",
                table: "TrackTrajectory");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TrackTrajectory");
        }
    }
}
