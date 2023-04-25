using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedResolvedFieldToBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Resolved",
                table: "Bugs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Resolved",
                table: "Bugs");
        }
    }
}
