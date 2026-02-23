using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnoJsonId_ToTrackAnnotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnnotationJsonId",
                table: "TrackAnnotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnotationJsonId",
                table: "TrackAnnotations");
        }
    }
}
