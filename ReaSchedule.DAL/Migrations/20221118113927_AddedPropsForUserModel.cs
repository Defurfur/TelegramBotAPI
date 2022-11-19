using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    public partial class AddedPropsForUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayNumberToUpdate",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayOfUpdate",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SubscriptionEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UpdateSchedule",
                table: "Users",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayNumberToUpdate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DayOfUpdate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SubscriptionEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdateSchedule",
                table: "Users");
        }
    }
}
