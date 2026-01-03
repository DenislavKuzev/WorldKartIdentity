using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldKartIdentity.Migrations
{
    /// <inheritdoc />
    public partial class AddPKtoBlogLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogLikes_Users_UserId",
                table: "BlogLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogLikes",
                table: "BlogLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BlogLikes");

            migrationBuilder.AddColumn<int>(
        name: "Id",
        table: "BlogLikes",
        type: "int",
        nullable: false,
        defaultValue: 0)
        .Annotation("SqlServer:Identity", "1, 1");


            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogLikes",
                table: "BlogLikes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BlogLikes_UserId",
                table: "BlogLikes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogLikes_Users_UserId",
                table: "BlogLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogLikes_Users_UserId",
                table: "BlogLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogLikes",
                table: "BlogLikes");

            migrationBuilder.DropIndex(
                name: "IX_BlogLikes_UserId",
                table: "BlogLikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BlogLikes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogLikes",
                table: "BlogLikes",
                columns: new[] { "UserId", "BlogId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlogLikes_Users_UserId",
                table: "BlogLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
