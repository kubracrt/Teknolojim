using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _Net.Migrations
{
    /// <inheritdoc />
    public partial class updatedViewEventService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ViewEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ViewEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
