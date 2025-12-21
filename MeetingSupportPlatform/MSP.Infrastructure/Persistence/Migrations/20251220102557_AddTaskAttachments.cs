using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAttachments_ProjectTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "ProjectTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a5b6c7d8-e9f0-4789-1234-56789abcdef6"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "af24742c-9dd8-4fa8-ab6a-3ba1b687683e", "AQAAAAIAAYagAAAAEEKR+yp/hVSXw0WgP1MA8W0rCb+Z8YpNx8HEbiUmn33wTC3sXO5NrZkTYs2gQMFwaQ==", "7c54c219-2543-4eb7-905c-feff7bbaa62e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b6c7d8e9-f0a1-4789-1234-56789abcdef7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dfede9ad-6a50-4f05-ab40-d415653fa42e", "AQAAAAIAAYagAAAAEMjKlDeqURX+EbAxk+985uYYVW5k/cjzw+CbPLVlwGfwMeIM4vnHW2e4Ckk3tsLWbA==", "bad37fe7-d5d3-4539-990b-28f0c81323eb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cf72542a-cb2e-403e-aaa2-52223be97edf", "AQAAAAIAAYagAAAAEALRwsb26SQcYvSstBl+7gPHa1BA7He9yy80Y7PUSWEGJciy4a/kX8f8RCRFR1ZF4A==", "0bb88086-76d5-452d-a10d-58fe50495447" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "89921342-96ca-43e8-8f6d-2fd6f9dbe5d8", "AQAAAAIAAYagAAAAEML88GzOa2eWo7MBAjlOeJy3FTh2CeAkXcGh2Yhu6+CcS3sOl660N50rzLQqAJ5iHQ==", "e5da197f-26f5-47b6-a86d-530a04e6fe7d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2768579-f549-47bd-b945-10625a9501da", "AQAAAAIAAYagAAAAEBHrIb66KY4wH1Cm5WUgJEG1AjCc2IdYvj5MI+qyEjUficuDbX7Lr5DApuqEM9QpmA==", "56835543-bc71-468b-977e-c24e8b749f57" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "93a9fc9b-de46-4ba0-b25d-34ed507097ce", "AQAAAAIAAYagAAAAENCBqco9rS0EIEe6RIqGd0Gz5OJeku7Nd+laawrADEU2L5lWijgr6jhWOctSfuczdQ==", "4045c6c6-bf6b-4a71-91f5-ec45509bbbf6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d2a5b3c4-d7e8-4789-1234-56789abcdef3"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f4b4c28a-60cd-4287-b633-f7c4b5658129", "AQAAAAIAAYagAAAAEN9uwImdm4xt1/7d8eqnivl9CQV4Zlv+kQG9xC3txzgy6sNvHnHSZpMa+POw4WkwOQ==", "768d6ec0-eccc-4bcc-8875-398f72599898" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3b6c7d8-a9f0-4789-1234-56789abcdef4"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8d6468ab-8553-45fb-b2f0-49470da03471", "AQAAAAIAAYagAAAAEFPEHQC521x9BpXp5zY6+JgtL9V47ospE54cfTB1oSklMgQzOwN0zakKb/rFnM/XIA==", "12e5be6e-3d7a-489b-a3d7-927db92a8da9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("f4c7d8e9-b1a2-4789-1234-56789abcdef5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "624bb411-b8ba-4e28-aa95-c87579d5ceac", "AQAAAAIAAYagAAAAEL8qwPdKpKyFee1k73+6M858Kkb3n4UZvVT0hwGwkJ90FEb8dostNczBMg0dM9i+IA==", "a8c518c2-22ef-4c13-b92e-849c98a2b000" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_TaskId",
                table: "TaskAttachments",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskAttachments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a5b6c7d8-e9f0-4789-1234-56789abcdef6"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2996245-bc5f-45f2-a3a2-2ee0282cf2a4", "AQAAAAIAAYagAAAAEJ9P5uEVpmoMa6950s1iSGDEmFJaTqZrkhBUPVPs8RzCvU3VUXD6v3j80JqaMMD4mw==", "c8e1df5e-5ac1-4004-ada7-a602a0f62ccc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b6c7d8e9-f0a1-4789-1234-56789abcdef7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5dd056da-c683-4647-950f-884778332efd", "AQAAAAIAAYagAAAAEOUvKu1usc555r33Fl105c2+Y2BOVQbgMuTrl77C9NerqRZfxKyvmPTxhkL3Z9at3Q==", "d3ba5100-4834-4311-a624-9ac2a11e9659" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1dac9191-b448-40d7-b03e-96d6149762c9", "AQAAAAIAAYagAAAAEFxQ0y2sSpxIDZ2R+jwAIaR4flvKEeKnD+rUih+D5/q5zPhYEHQDN8Mfckzd8wsyMw==", "668cb7d3-04f3-4fed-9da9-f5c782e76bec" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "217d2aef-0f55-4e72-b6d4-ed7dcbc6529a", "AQAAAAIAAYagAAAAEG/TsMYMFYloVbH/oq02kDNTNhip6/ig2UAka7PptFifYGQCPz7dXrM6Sh9Cy3/yyg==", "13551c31-1e4c-421e-9d10-eb1940ef2251" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cb9ab1b0-37dd-431f-b234-67587004beeb", "AQAAAAIAAYagAAAAEIIWurJ3sJuvnejJVxGGFfZp15HVnDdR7VwgEwWAUeR8pN2I6ftZ4sPlJwqPIZ+M6Q==", "95fc7151-3d1a-4370-b80c-519efe31d1c8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "48a00da7-f3be-4c9f-a074-ce96aebfd708", "AQAAAAIAAYagAAAAEA5rZfUqE7qlca3qSuVtwY0LgLWwz/oeOY6eB+NzpF7PQ4irpeietwT2KYpyvMBuSA==", "c9b6cd98-4d71-4027-997a-ede5b8d2ae17" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d2a5b3c4-d7e8-4789-1234-56789abcdef3"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "53f5506e-3f1a-429a-9357-8de966084271", "AQAAAAIAAYagAAAAENdR1qiCmPOiS3W8/jXLRv5Tp0FqUNHfCS0P41STpsPauNcR2qrlnIQYiXjHJ/94zg==", "a25d3944-66ca-474e-9322-08acb022733b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3b6c7d8-a9f0-4789-1234-56789abcdef4"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b7c4c32f-303a-4b48-a945-ccbe2a7b2571", "AQAAAAIAAYagAAAAEHS41ov/CcQTFl0F2mqYZTqd73UyBz8KxM2Ob5P+KsRdsJ7+ktcYoaDWVNfPdm3PGg==", "7523bc3a-eeca-43f5-9cb6-fc38f6f4db7c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("f4c7d8e9-b1a2-4789-1234-56789abcdef5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fdf2d046-52f8-46cf-a9fb-b7b02693d535", "AQAAAAIAAYagAAAAEDwp7ghwK/ZBPNAQ08a6VLG5Qx0zOM5L01/KnOaxG3yBUuRcGerioOmI+sj43xRt4w==", "08d04ab1-29ea-4a97-8bc7-a103ff58f7e2" });
        }
    }
}
