using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoMapAPI.Migrations
{
    /// <inheritdoc />
    public partial class ControlVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_UserId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UserId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Photos");

            migrationBuilder.CreateTable(
                name: "PhotoUser",
                columns: table => new
                {
                    LikedByUsersId = table.Column<string>(type: "text", nullable: false),
                    LikedPhotosUId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoUser", x => new { x.LikedByUsersId, x.LikedPhotosUId });
                    table.ForeignKey(
                        name: "FK_PhotoUser_AspNetUsers_LikedByUsersId",
                        column: x => x.LikedByUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoUser_Photos_LikedPhotosUId",
                        column: x => x.LikedPhotosUId,
                        principalTable: "Photos",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoUser_LikedPhotosUId",
                table: "PhotoUser",
                column: "LikedPhotosUId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoUser");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Photos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
