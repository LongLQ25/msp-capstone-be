using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_AspNetUsers_CreatedById",
                table: "Meeting");

            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Milestone_MilestoneId",
                table: "Meeting");

            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Project_ProjectId",
                table: "Meeting");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingUsers_Meeting_MeetingId",
                table: "MeetingUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Todo_Meeting_MeetingId",
                table: "Todo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting");

            migrationBuilder.RenameTable(
                name: "Meeting",
                newName: "Meetings");

            migrationBuilder.RenameIndex(
                name: "IX_Meeting_ProjectId",
                table: "Meetings",
                newName: "IX_Meetings_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Meeting_MilestoneId",
                table: "Meetings",
                newName: "IX_Meetings_MilestoneId");

            migrationBuilder.RenameIndex(
                name: "IX_Meeting_CreatedById",
                table: "Meetings",
                newName: "IX_Meetings_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "797282f1-d50a-4586-9a69-56ab8d2b14b1", "AQAAAAIAAYagAAAAEBhpd+tRlBRsUiauwEfalTK6/dEzr9E6AyEYS8oaRkT6yQphZ5D8WIb1YLYxg5gtrQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e78bcdb1-5c6c-4fc7-8bb1-59791f46964d", "AQAAAAIAAYagAAAAEPNt65EEQDBYWvNzRYmAwdKkQNJ+K9ziEQXXb2GufxDlV+DjsTsDp/GmWu18+61xrg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e28e3ce6-a8cc-4a40-9bc5-9495a6327033", "AQAAAAIAAYagAAAAEHZquqKsBt1EskTR/4gb4Ef2Gg3lL+esElYX+eKn3hpbfU3Xgm5hko6VU+QizJmQPQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3cad4290-dce4-4a32-ad20-a830eecda865", "AQAAAAIAAYagAAAAEJ5fJakfwKcWqOVjcksuXaGwy1vBYGUM4ecF+aj7uMf4AHpWavyeYCoqMZElTun1Lg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_CreatedById",
                table: "Meetings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Milestone_MilestoneId",
                table: "Meetings",
                column: "MilestoneId",
                principalTable: "Milestone",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Project_ProjectId",
                table: "Meetings",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingUsers_Meetings_MeetingId",
                table: "MeetingUsers",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todo_Meetings_MeetingId",
                table: "Todo",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_CreatedById",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Milestone_MilestoneId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Project_ProjectId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingUsers_Meetings_MeetingId",
                table: "MeetingUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Todo_Meetings_MeetingId",
                table: "Todo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings");

            migrationBuilder.RenameTable(
                name: "Meetings",
                newName: "Meeting");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_ProjectId",
                table: "Meeting",
                newName: "IX_Meeting_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_MilestoneId",
                table: "Meeting",
                newName: "IX_Meeting_MilestoneId");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_CreatedById",
                table: "Meeting",
                newName: "IX_Meeting_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7149faee-1681-4b98-be71-1c4b7a27e80d", "AQAAAAIAAYagAAAAEFw7L449o6jE9rfeVDVj6kvoMm/FcxtrJUjy8+xGRnga5gf1LdMJobSE2H0QfUEauA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6a121d17-0108-49c0-8709-3c9446634144", "AQAAAAIAAYagAAAAEMHawpxbEvW17fAFqXj4cMdom0rQgkQNROxLQhdmOp+K98wL+aTchj1TMRLpqAFm2g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5bc361f1-d36f-4c3b-bd28-3a40d834df7d", "AQAAAAIAAYagAAAAEKO3EHeKD7dpJbfF+oArtF9KDvVYe1cuip7EVyh7ohCe6dZfK0aRFQEd3gV8um8YtA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "db053d07-1074-457f-9c7b-7cb02d415079", "AQAAAAIAAYagAAAAENgpYpMVonrZlS+B7vy4XBjr8BKIuYDL57b5JDtmEuoyzuyhdzqu1kSPT86eBnh2Qw==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_AspNetUsers_CreatedById",
                table: "Meeting",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Milestone_MilestoneId",
                table: "Meeting",
                column: "MilestoneId",
                principalTable: "Milestone",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Project_ProjectId",
                table: "Meeting",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingUsers_Meeting_MeetingId",
                table: "MeetingUsers",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todo_Meeting_MeetingId",
                table: "Todo",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
