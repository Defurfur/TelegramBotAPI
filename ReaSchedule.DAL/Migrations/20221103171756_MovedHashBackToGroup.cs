﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    public partial class MovedHashBackToGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "ScheduleWeeks");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "ReaGroups",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "ReaGroups");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "ScheduleWeeks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
