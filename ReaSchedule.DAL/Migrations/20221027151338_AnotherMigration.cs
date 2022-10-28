using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    public partial class AnotherMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReaClasses_ScheduleDays_ScheduleDayId",
                table: "ReaClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleDays",
                table: "ScheduleDays");

            migrationBuilder.DropColumn(
                name: "ClassTimePeriod",
                table: "ReaClasses");

            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "ScheduleDays");

            migrationBuilder.RenameTable(
                name: "ScheduleDays",
                newName: "ScheduleDay");

            migrationBuilder.RenameColumn(
                name: "Group",
                table: "ReaClasses",
                newName: "Subgroup");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleDayId",
                table: "ReaClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassType",
                table: "ReaClasses",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "OrdinalNumber",
                table: "ReaClasses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "ScheduleDay",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "ScheduleDay",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeekName",
                table: "ScheduleDay",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleWeekId",
                table: "ScheduleDay",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleDay",
                table: "ScheduleDay",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ReaGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupName = table.Column<string>(type: "text", nullable: false),
                    HashSum = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReaGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleWeeks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReaGroupId = table.Column<int>(type: "integer", nullable: false),
                    WeekStart = table.Column<DateOnly>(type: "date", nullable: false),
                    WeekEnd = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleWeeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleWeeks_ReaGroups_ReaGroupId",
                        column: x => x.ReaGroupId,
                        principalTable: "ReaGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDay_ScheduleWeekId",
                table: "ScheduleDay",
                column: "ScheduleWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleWeeks_ReaGroupId",
                table: "ScheduleWeeks",
                column: "ReaGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReaClasses_ScheduleDay_ScheduleDayId",
                table: "ReaClasses",
                column: "ScheduleDayId",
                principalTable: "ScheduleDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleDay_ScheduleWeeks_ScheduleWeekId",
                table: "ScheduleDay",
                column: "ScheduleWeekId",
                principalTable: "ScheduleWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReaClasses_ScheduleDay_ScheduleDayId",
                table: "ReaClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDay_ScheduleWeeks_ScheduleWeekId",
                table: "ScheduleDay");

            migrationBuilder.DropTable(
                name: "ScheduleWeeks");

            migrationBuilder.DropTable(
                name: "ReaGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleDay",
                table: "ScheduleDay");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleDay_ScheduleWeekId",
                table: "ScheduleDay");

            migrationBuilder.DropColumn(
                name: "OrdinalNumber",
                table: "ReaClasses");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "ScheduleDay");

            migrationBuilder.DropColumn(
                name: "DayOfWeekName",
                table: "ScheduleDay");

            migrationBuilder.DropColumn(
                name: "ScheduleWeekId",
                table: "ScheduleDay");

            migrationBuilder.RenameTable(
                name: "ScheduleDay",
                newName: "ScheduleDays");

            migrationBuilder.RenameColumn(
                name: "Subgroup",
                table: "ReaClasses",
                newName: "Group");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleDayId",
                table: "ReaClasses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ClassType",
                table: "ReaClasses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ClassTimePeriod",
                table: "ReaClasses",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "ScheduleDays",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "ScheduleDays",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleDays",
                table: "ScheduleDays",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReaClasses_ScheduleDays_ScheduleDayId",
                table: "ReaClasses",
                column: "ScheduleDayId",
                principalTable: "ScheduleDays",
                principalColumn: "Id");
        }
    }
}
