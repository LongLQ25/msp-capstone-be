using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewerToProjectTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReviewerId",
                table: "ProjectTasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a5b6c7d8-e9f0-4789-1234-56789abcdef6"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "17ea0fd7-25fc-46d7-8651-4205d192f9a4", "AQAAAAIAAYagAAAAELGZgy+LFDYDAkYzke365z8xI7FeAmJtdJdYgrcT9Yk7d4ZrCADpZZ+Y0YZxnFc59A==", "173a3834-4cf1-4f18-bab9-c507908d350c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b6c7d8e9-f0a1-4789-1234-56789abcdef7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "95aa81e4-144d-4a59-a2fb-cd55eabcccba", "AQAAAAIAAYagAAAAEKuaUWgfl5E2e8lmPvDFuwLNk2zK/oh0bl8zcIUV02MOrO5R8b54AA7eMyG6DJOYUA==", "d348dbd6-298e-4f3d-aaeb-e9e7a60fd515" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "79424eea-65f2-44ad-a217-98008705ba8e", "AQAAAAIAAYagAAAAEDTqa7Ovccl70TjP95telRhdyJFLfPflIJVLgNYnBxBfjxhj2fWIfkqkHY5VOYwPCQ==", "1dfd4c7d-0d79-44af-bf3f-ead4e7b11343" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bedc9324-d05a-4160-85d8-a89440b2fb77", "AQAAAAIAAYagAAAAECqa+NcXq37nwxbzk5Vgq86fq+rNUtZz0HoCQdhcJLTzu2R3QpDRrbLNw6ZxH3NjyA==", "7a4dfea4-6a65-42b0-8f83-2cffd5073f47" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8ece264d-10c1-471d-a881-0f99c7364f4f", "AQAAAAIAAYagAAAAEHFJIAZ1LS0ODkjT97JNVDOrGXWjTYPKdoeWi5vPUD6fQN42vdzvurt4WZl8RQorsg==", "cf4500ba-0b20-429f-aa57-bde0557aba9f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84b622de-7225-4830-ae45-2699ffd4c0c1", "AQAAAAIAAYagAAAAEEXU8bQIOfuQey82dCRuGS06PjAUcIgnxWE5u42NjTvfEMCi3eEIOYRgrFTWGGY0ew==", "31675d89-03b8-491e-b5c2-7ec0bc637e31" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d2a5b3c4-d7e8-4789-1234-56789abcdef3"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a762ae92-973b-4994-990c-b9b9d3789eb1", "AQAAAAIAAYagAAAAEJlid4OZ7JBc3tJVNzznoxseqsxnGO+NTgy2R2Q5Nt8U9spu/mmC3NcGlng9wn3jWQ==", "46fab13c-f5e1-4a86-9464-35927e1d0916" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3b6c7d8-a9f0-4789-1234-56789abcdef4"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "34000a2b-0173-4090-bab3-01d0a3f3813c", "AQAAAAIAAYagAAAAEJw7vj045Ut9jhUXSWfPJNfEljKQhoB7HNjuMGLQ2xLvLCwtEsRqYsaYU1zQuW+mwA==", "be5dbd1e-5c13-4931-9430-4232af9534c4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("f4c7d8e9-b1a2-4789-1234-56789abcdef5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "782f84c8-17fd-4d52-bd57-1b8b1faf522b", "AQAAAAIAAYagAAAAEJzPT86iTg/3WqpMkP3Q6qR5ntHt6HyzYP95IniXFEnGd5R3SSeZr7zzpDYuuXy5MQ==", "cf6d215f-f4eb-4413-aa1f-c3745b1b74ef" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_ReviewerId",
                table: "ProjectTasks",
                column: "ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_ReviewerId",
                table: "ProjectTasks",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_ReviewerId",
                table: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_ReviewerId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "ProjectTasks");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a5b6c7d8-e9f0-4789-1234-56789abcdef6"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a7237d28-874e-419a-939a-83501b18ca33", "AQAAAAIAAYagAAAAEJIlUrS4epydsgeBi2TZu6DgjTCzw6ynLYDA3AvLtNPOUMgf+Qo6f/tqXyhQv8rlRA==", "832b0079-e2ec-4217-9daf-cf2afdd77918" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b6c7d8e9-f0a1-4789-1234-56789abcdef7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "99f3c792-d349-43ae-bf19-b7876df692a3", "AQAAAAIAAYagAAAAELMa5yZymzsqMiltry8m79TTky1y3l1OcPcc6j2+3G1J/ewYotVpF56kGzDBHTd8yw==", "060e789a-3004-4e07-97c0-243587490393" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b4370a17-1616-49e5-ae01-87aba7c4c7eb", "AQAAAAIAAYagAAAAEDJ04ad26da+4Jlvel+3ismDh4YG9pFGfa/RreO9s3/05al3cPzgyeYDaCttB65Sow==", "cf38a86e-05ae-48ea-9084-e45e51828eca" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "33be5b23-4b70-4ffa-9b39-032c564040dc", "AQAAAAIAAYagAAAAEPgsU7dapy2mImVKn1qViHdmnmeVY1yX6R5uSndk1L+68eswoOYblQdwSf5EMSubBQ==", "565fde08-2b7f-46a5-a084-60a6f1d08ed5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "702541bf-1863-4815-8ad3-2031cb6d410a", "AQAAAAIAAYagAAAAEIKlHiQWa7nQVSfeF/oCzlx0FeL9vvEbP1rOsLG8hoC6r9MJSQtQ6jSs4gc+BfxdBg==", "170675fa-855c-4631-9859-a0a5b608ac51" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "479ea9fb-ee92-4862-b282-46219b37fc67", "AQAAAAIAAYagAAAAEDtJhZsBZiqodyS5d9x220ZZsPhZsWT9B7Ecis4JHAJDOltcFVD62eRz1OfouqHgGw==", "8f2cb858-bf00-4201-b92f-217029b9f85c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d2a5b3c4-d7e8-4789-1234-56789abcdef3"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a088a8ae-8edb-49bc-8399-ae1536276f6c", "AQAAAAIAAYagAAAAENPTXlwLa7AzMW0PHaVo8l8KZfRHywolyw0ohpWsQWSAddKyHr/Yaux+YqFH7m/BCQ==", "b3cc8599-a22c-449b-b9ea-65289efaae3c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3b6c7d8-a9f0-4789-1234-56789abcdef4"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0bef7cfa-f791-4a93-b08a-0d01be81e9bd", "AQAAAAIAAYagAAAAEPOs6hqwcM/Pc/NQtfpTCAzKR3NRx0BNLp4Xl2nCmyU2fZ9Ioo+NFh8oKKJszUDD5g==", "7f9c2725-cb93-4965-9e3c-c4e9f9a6e0b9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("f4c7d8e9-b1a2-4789-1234-56789abcdef5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c929702c-8345-44ee-9e4f-af33244d403d", "AQAAAAIAAYagAAAAEG5+8GhKzfzvSjeL5Fm/Vwie5ytmDDt7tbrR6Cpkfqt+y0lstC0qqd5Tn7H71HfJ3w==", "2ca3ccc1-19fe-4624-abec-c49fd5152366" });
        }
    }
}
