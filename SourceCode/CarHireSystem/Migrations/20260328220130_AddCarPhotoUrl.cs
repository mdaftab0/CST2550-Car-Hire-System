using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarHireSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCarPhotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Cars");
        }
    }
}
