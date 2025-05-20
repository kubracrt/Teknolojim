using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _Net.Migrations
{
    /// <inheritdoc />
    public partial class addViewEventService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ViewEvents_Users_UserId",
                table: "ViewEvents");

            migrationBuilder.DropIndex(
                name: "IX_ViewEvents_UserId",
                table: "ViewEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ViewEvents_UserId",
                table: "ViewEvents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewEvents_Users_UserId",
                table: "ViewEvents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
