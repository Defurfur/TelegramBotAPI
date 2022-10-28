using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    public partial class AddedScheduleDaysDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReaClasses_ScheduleDay_ScheduleDayId",
                table: "ReaClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDay_ScheduleWeeks_ScheduleWeekId",
                table: "ScheduleDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleDay",
                table: "ScheduleDay");

            migrationBuilder.RenameTable(
                name: "ScheduleDay",
                newName: "ScheduleDays");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleDay_ScheduleWeekId",
                table: "ScheduleDays",
                newName: "IX_ScheduleDays_ScheduleWeekId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleDays",
                table: "ScheduleDays",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReaClasses_ScheduleDays_ScheduleDayId",
                table: "ReaClasses",
                column: "ScheduleDayId",
                principalTable: "ScheduleDays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleDays_ScheduleWeeks_ScheduleWeekId",
                table: "ScheduleDays",
                column: "ScheduleWeekId",
                principalTable: "ScheduleWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReaClasses_ScheduleDays_ScheduleDayId",
                table: "ReaClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleDays_ScheduleWeeks_ScheduleWeekId",
                table: "ScheduleDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleDays",
                table: "ScheduleDays");

            migrationBuilder.RenameTable(
                name: "ScheduleDays",
                newName: "ScheduleDay");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleDays_ScheduleWeekId",
                table: "ScheduleDay",
                newName: "IX_ScheduleDay_ScheduleWeekId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleDay",
                table: "ScheduleDay",
                column: "Id");

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
    }
}
