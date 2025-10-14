using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_ProjectTask_TaskId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_AspNetUsers_OwnerId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_Project_ProjectId",
                table: "Document");

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
                name: "FK_Milestone_AspNetUsers_UserId",
                table: "Milestone");

            migrationBuilder.DropForeignKey(
                name: "FK_Milestone_Project_ProjectId",
                table: "Milestone");

            migrationBuilder.DropForeignKey(
                name: "FK_MilestoneTasks_Milestone_MilestoneId",
                table: "MilestoneTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_MilestoneTasks_ProjectTask_ProjectTaskId",
                table: "MilestoneTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_AspNetUsers_CreatedById",
                table: "Package");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageFeatures_Feature_FeatureId",
                table: "PackageFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageFeatures_Package_PackageId",
                table: "PackageFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_CreatedById",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_OwnerId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_AspNetUsers_MemberId",
                table: "ProjectMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_Project_ProjectId",
                table: "ProjectMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_AspNetUsers_UserId",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_Project_ProjectId",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_Todo_TodoId",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_AspNetUsers_UserId",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Package_PackageId",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Todo_AspNetUsers_UserId",
                table: "Todo");

            migrationBuilder.DropForeignKey(
                name: "FK_Todo_Meeting_MeetingId",
                table: "Todo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Todo",
                table: "Todo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTask",
                table: "ProjectTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMember",
                table: "ProjectMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Package",
                table: "Package");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Milestone",
                table: "Milestone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feature",
                table: "Feature");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Todo",
                newName: "Todos");

            migrationBuilder.RenameTable(
                name: "Subscription",
                newName: "Subscriptions");

            migrationBuilder.RenameTable(
                name: "ProjectTask",
                newName: "ProjectTasks");

            migrationBuilder.RenameTable(
                name: "ProjectMember",
                newName: "ProjectMembers");

            migrationBuilder.RenameTable(
                name: "Project",
                newName: "Projects");

            migrationBuilder.RenameTable(
                name: "Package",
                newName: "Packages");

            migrationBuilder.RenameTable(
                name: "Milestone",
                newName: "Milestones");

            migrationBuilder.RenameTable(
                name: "Meeting",
                newName: "Meetings");

            migrationBuilder.RenameTable(
                name: "Feature",
                newName: "Features");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Todo_UserId",
                table: "Todos",
                newName: "IX_Todos_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Todo_MeetingId",
                table: "Todos",
                newName: "IX_Todos_MeetingId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_UserId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_PackageId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTask_UserId",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTask_TodoId",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_TodoId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTask_ProjectId",
                table: "ProjectTasks",
                newName: "IX_ProjectTasks_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMember_ProjectId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMember_MemberId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_OwnerId",
                table: "Projects",
                newName: "IX_Projects_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_CreatedById",
                table: "Projects",
                newName: "IX_Projects_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Package_CreatedById",
                table: "Packages",
                newName: "IX_Packages_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Milestone_UserId",
                table: "Milestones",
                newName: "IX_Milestones_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Milestone_ProjectId",
                table: "Milestones",
                newName: "IX_Milestones_ProjectId");

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

            migrationBuilder.RenameIndex(
                name: "IX_Document_ProjectId",
                table: "Documents",
                newName: "IX_Documents_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_OwnerId",
                table: "Documents",
                newName: "IX_Documents_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_TaskId",
                table: "Comments",
                newName: "IX_Comments_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Todos",
                table: "Todos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTasks",
                table: "ProjectTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Projects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Packages",
                table: "Packages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Milestones",
                table: "Milestones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Features",
                table: "Features",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "37ad7c16-cb89-4711-a63a-a130ff84e108", "AQAAAAIAAYagAAAAEP2fiz0RYSTE2/QlNsnJTIJsX2qE1pvyyEvzyuQ9FoIuDaoMJ5rjXwKrB0H3atDyvA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "09e77445-b561-4657-bdf5-3dbbd9000f5c", "AQAAAAIAAYagAAAAEJEDD5EXgkf5p2KZytDk39/ETHtHZXAtWh0TwchDUnvIzhoMPyMqHwemXIfDU/7kQg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "99931516-8b65-4370-abd7-8e9db860d414", "AQAAAAIAAYagAAAAEHVJFZGpE1KRargxMWwsV/lrNXQWpKN+J5ju10w00C6mP44J40ykN9OYlgGOKBBieg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d7cdaa35-5330-4dd6-b5bd-8bea5f3a6d46", "AQAAAAIAAYagAAAAEPBQl1tsyJKKslGkoGd7THfsv61irR6OY4e+BEYM/uZM7ojyhPtg7fr18eCZQOGpTw==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ProjectTasks_TaskId",
                table: "Comments",
                column: "TaskId",
                principalTable: "ProjectTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_OwnerId",
                table: "Documents",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Projects_ProjectId",
                table: "Documents",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_CreatedById",
                table: "Meetings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Milestones_MilestoneId",
                table: "Meetings",
                column: "MilestoneId",
                principalTable: "Milestones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Projects_ProjectId",
                table: "Meetings",
                column: "ProjectId",
                principalTable: "Projects",
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
                name: "FK_Milestones_AspNetUsers_UserId",
                table: "Milestones",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Milestones_Projects_ProjectId",
                table: "Milestones",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MilestoneTasks_Milestones_MilestoneId",
                table: "MilestoneTasks",
                column: "MilestoneId",
                principalTable: "Milestones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MilestoneTasks_ProjectTasks_ProjectTaskId",
                table: "MilestoneTasks",
                column: "ProjectTaskId",
                principalTable: "ProjectTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageFeatures_Features_FeatureId",
                table: "PackageFeatures",
                column: "FeatureId",
                principalTable: "Features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageFeatures_Packages_PackageId",
                table: "PackageFeatures",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_AspNetUsers_CreatedById",
                table: "Packages",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_MemberId",
                table: "ProjectMembers",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedById",
                table: "Projects",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_UserId",
                table: "ProjectTasks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_Todos_TodoId",
                table: "ProjectTasks",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_AspNetUsers_UserId",
                table: "Subscriptions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Packages_PackageId",
                table: "Subscriptions",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_UserId",
                table: "Todos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Meetings_MeetingId",
                table: "Todos",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ProjectTasks_TaskId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_OwnerId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Projects_ProjectId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_CreatedById",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Milestones_MilestoneId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Projects_ProjectId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingUsers_Meetings_MeetingId",
                table: "MeetingUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Milestones_AspNetUsers_UserId",
                table: "Milestones");

            migrationBuilder.DropForeignKey(
                name: "FK_Milestones_Projects_ProjectId",
                table: "Milestones");

            migrationBuilder.DropForeignKey(
                name: "FK_MilestoneTasks_Milestones_MilestoneId",
                table: "MilestoneTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_MilestoneTasks_ProjectTasks_ProjectTaskId",
                table: "MilestoneTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageFeatures_Features_FeatureId",
                table: "PackageFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageFeatures_Packages_PackageId",
                table: "PackageFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_AspNetUsers_CreatedById",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_AspNetUsers_MemberId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_UserId",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Projects_ProjectId",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_Todos_TodoId",
                table: "ProjectTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_AspNetUsers_UserId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Packages_PackageId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_UserId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Meetings_MeetingId",
                table: "Todos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Todos",
                table: "Todos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTasks",
                table: "ProjectTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Packages",
                table: "Packages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Milestones",
                table: "Milestones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Features",
                table: "Features");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Todos",
                newName: "Todo");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "Subscription");

            migrationBuilder.RenameTable(
                name: "ProjectTasks",
                newName: "ProjectTask");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Project");

            migrationBuilder.RenameTable(
                name: "ProjectMembers",
                newName: "ProjectMember");

            migrationBuilder.RenameTable(
                name: "Packages",
                newName: "Package");

            migrationBuilder.RenameTable(
                name: "Milestones",
                newName: "Milestone");

            migrationBuilder.RenameTable(
                name: "Meetings",
                newName: "Meeting");

            migrationBuilder.RenameTable(
                name: "Features",
                newName: "Feature");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Todos_UserId",
                table: "Todo",
                newName: "IX_Todo_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Todos_MeetingId",
                table: "Todo",
                newName: "IX_Todo_MeetingId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscription",
                newName: "IX_Subscription_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_PackageId",
                table: "Subscription",
                newName: "IX_Subscription_PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_UserId",
                table: "ProjectTask",
                newName: "IX_ProjectTask_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_TodoId",
                table: "ProjectTask",
                newName: "IX_ProjectTask_TodoId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTasks_ProjectId",
                table: "ProjectTask",
                newName: "IX_ProjectTask_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_OwnerId",
                table: "Project",
                newName: "IX_Project_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CreatedById",
                table: "Project",
                newName: "IX_Project_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_ProjectId",
                table: "ProjectMember",
                newName: "IX_ProjectMember_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_MemberId",
                table: "ProjectMember",
                newName: "IX_ProjectMember_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Packages_CreatedById",
                table: "Package",
                newName: "IX_Package_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Milestones_UserId",
                table: "Milestone",
                newName: "IX_Milestone_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Milestones_ProjectId",
                table: "Milestone",
                newName: "IX_Milestone_ProjectId");

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

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ProjectId",
                table: "Document",
                newName: "IX_Document_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_OwnerId",
                table: "Document",
                newName: "IX_Document_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TaskId",
                table: "Comment",
                newName: "IX_Comment_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Todo",
                table: "Todo",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTask",
                table: "ProjectTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMember",
                table: "ProjectMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Package",
                table: "Package",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Milestone",
                table: "Milestone",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feature",
                table: "Feature",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
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
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_ProjectTask_TaskId",
                table: "Comment",
                column: "TaskId",
                principalTable: "ProjectTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_AspNetUsers_OwnerId",
                table: "Document",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Project_ProjectId",
                table: "Document",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Milestone_AspNetUsers_UserId",
                table: "Milestone",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Milestone_Project_ProjectId",
                table: "Milestone",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MilestoneTasks_Milestone_MilestoneId",
                table: "MilestoneTasks",
                column: "MilestoneId",
                principalTable: "Milestone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MilestoneTasks_ProjectTask_ProjectTaskId",
                table: "MilestoneTasks",
                column: "ProjectTaskId",
                principalTable: "ProjectTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Package_AspNetUsers_CreatedById",
                table: "Package",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageFeatures_Feature_FeatureId",
                table: "PackageFeatures",
                column: "FeatureId",
                principalTable: "Feature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageFeatures_Package_PackageId",
                table: "PackageFeatures",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_CreatedById",
                table: "Project",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_OwnerId",
                table: "Project",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_AspNetUsers_MemberId",
                table: "ProjectMember",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_Project_ProjectId",
                table: "ProjectMember",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_AspNetUsers_UserId",
                table: "ProjectTask",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_Project_ProjectId",
                table: "ProjectTask",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_Todo_TodoId",
                table: "ProjectTask",
                column: "TodoId",
                principalTable: "Todo",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_AspNetUsers_UserId",
                table: "Subscription",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Package_PackageId",
                table: "Subscription",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Todo_AspNetUsers_UserId",
                table: "Todo",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
