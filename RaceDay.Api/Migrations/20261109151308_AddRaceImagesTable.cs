using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDay.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRaceImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceEvents_AspNetUsers_ApplicationUserId",
                table: "RaceEvents");

            migrationBuilder.DropIndex(
                name: "IX_RaceEvents_ApplicationUserId",
                table: "RaceEvents");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "RaceEvents");

            migrationBuilder.RenameColumn(
                name: "profileRole",
                table: "AspNetUsers",
                newName: "ProfileRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileRole",
                table: "AspNetUsers",
                newName: "profileRole");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "RaceEvents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaceEvents_ApplicationUserId",
                table: "RaceEvents",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceEvents_AspNetUsers_ApplicationUserId",
                table: "RaceEvents",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
