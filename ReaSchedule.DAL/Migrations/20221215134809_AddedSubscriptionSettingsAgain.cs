using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaSchedule.DAL.Migrations
{
    public partial class AddedSubscriptionSettingsAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionSettings_Users_UserId",
                table: "SubscriptionSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionSettings",
                table: "SubscriptionSettings");

            migrationBuilder.RenameTable(
                name: "SubscriptionSettings",
                newName: "Settings");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionSettings_UserId",
                table: "Settings",
                newName: "IX_Settings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                table: "Settings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_Users_UserId",
                table: "Settings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Settings_Users_UserId",
                table: "Settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                table: "Settings");

            migrationBuilder.RenameTable(
                name: "Settings",
                newName: "SubscriptionSettings");

            migrationBuilder.RenameIndex(
                name: "IX_Settings_UserId",
                table: "SubscriptionSettings",
                newName: "IX_SubscriptionSettings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionSettings",
                table: "SubscriptionSettings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionSettings_Users_UserId",
                table: "SubscriptionSettings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
