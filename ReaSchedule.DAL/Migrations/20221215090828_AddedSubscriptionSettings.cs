using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    public partial class AddedSubscriptionSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ReaGroups_ReaGroupId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ReaGroupId",
                table: "Users");

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

            migrationBuilder.CreateTable(
                name: "SubscriptionSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateSchedule = table.Column<int>(type: "integer", nullable: false),
                    DayOfUpdate = table.Column<int>(type: "integer", nullable: true),
                    DayAmountToUpdate = table.Column<int>(type: "integer", nullable: false),
                    TimeOfDay = table.Column<int>(type: "integer", nullable: false),
                    WeekToSend = table.Column<int>(type: "integer", nullable: false),
                    IncludeToday = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionSettings_UserId",
                table: "SubscriptionSettings",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionSettings");

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

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReaGroupId",
                table: "Users",
                column: "ReaGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ReaGroups_ReaGroupId",
                table: "Users",
                column: "ReaGroupId",
                principalTable: "ReaGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
