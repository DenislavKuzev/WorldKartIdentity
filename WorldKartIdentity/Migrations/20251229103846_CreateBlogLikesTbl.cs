using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class CreateBlogLikesTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "BlogLikes", columns: table => new { Id = table.Column<int>("int", nullable: false), UserId = table.Column<string>(type: "nvarchar(450)", nullable: false), BlogId = table.Column<int>(type: "int", nullable: false) }, constraints: table =>
            {
                table.PrimaryKey("PK_BlogLikes", x => x.Id).Annotation("SqlServer:Identity", "1, 1");
                table.ForeignKey(name: "FK_BlogLikes_BlogPosts_BlogId", column: x => x.BlogId, principalTable: "BlogPosts", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey(name: "FK_BlogLikes_Users_UserId", column: x => x.UserId, principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            });
            migrationBuilder.CreateIndex(name: "IX_BlogLikes_BlogId", table: "BlogLikes", column: "BlogId");
            migrationBuilder.CreateIndex(
                name: "IX_BlogLikes_UserId_BlogId",
                table: "BlogLikes",
                columns: new[] { "UserId", "BlogId" },
                unique: true
            ); // prevent duplicate likes by the same user on the same blog
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogLikes");
        }
    }
}
