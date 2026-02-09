using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class EditTrackAnnotationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JsonContent",
                table: "TrackAnnotations",
                newName: "AnnotationJson");

            migrationBuilder.AddColumn<string>(
                name: "UserAuthData",
                table: "TrackAnnotations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TrackAnnotations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_UserId",
                table: "TrackAnnotations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackAnnotations_Users_UserId",
                table: "TrackAnnotations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackAnnotations_Users_UserId",
                table: "TrackAnnotations");

            migrationBuilder.DropIndex(
                name: "IX_TrackAnnotations_UserId",
                table: "TrackAnnotations");

            migrationBuilder.DropColumn(
                name: "UserAuthData",
                table: "TrackAnnotations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TrackAnnotations");


            migrationBuilder.RenameColumn(
                name: "AnnotationJson",
                table: "TrackAnnotations",
                newName: "JsonContent");
        }
    }
}
