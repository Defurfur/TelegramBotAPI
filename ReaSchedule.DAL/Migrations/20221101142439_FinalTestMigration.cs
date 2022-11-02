using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    public partial class FinalTestMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassElementId",
                table: "ReaClasses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassElementId",
                table: "ReaClasses");
        }
    }
}
