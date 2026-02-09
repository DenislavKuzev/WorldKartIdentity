using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class CreateTrackAnnotationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackAnnotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackId = table.Column<int>(type: "int", nullable: false),
                    JsonContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAnnotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackAnnotations_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackAnnotations_Tracks_TrackId1",
                        column: x => x.TrackId1,
                        principalTable: "Tracks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_TrackId",
                table: "TrackAnnotations",
                column: "TrackId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackAnnotations_TrackId1",
                table: "TrackAnnotations",
                column: "TrackId1",
                unique: true,
                filter: "[TrackId1] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackAnnotations");
        }
    }
}
