using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoMapAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avatar_AspNetUsers_UserId",
                table: "Avatar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Avatar",
                table: "Avatar");

            migrationBuilder.RenameTable(
                name: "Avatar",
                newName: "Avatars");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Avatars",
                table: "Avatars",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avatars_AspNetUsers_UserId",
                table: "Avatars",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avatars_AspNetUsers_UserId",
                table: "Avatars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Avatars",
                table: "Avatars");

            migrationBuilder.RenameTable(
                name: "Avatars",
                newName: "Avatar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Avatar",
                table: "Avatar",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avatar_AspNetUsers_UserId",
                table: "Avatar",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
