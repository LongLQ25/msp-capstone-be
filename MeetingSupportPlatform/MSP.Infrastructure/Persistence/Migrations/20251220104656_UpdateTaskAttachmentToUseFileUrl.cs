using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskAttachmentToUseFileUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "TaskAttachments");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "TaskAttachments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "TaskAttachments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "TaskAttachments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "TaskAttachments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a5b6c7d8-e9f0-4789-1234-56789abcdef6"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b54cffe2-2e5c-4433-868c-511dfb964ad3", "AQAAAAIAAYagAAAAEFRZsAcVA0mO04wljKvM00gXg0ZwsTztDL6Mj7zLwHJJiSvRp7aBXR1RAxyiddeslw==", "ddb01c5b-3d90-4074-be55-790fd62a2abc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b6c7d8e9-f0a1-4789-1234-56789abcdef7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "51c65b04-6542-4e68-93b7-2b7b1ab4ab10", "AQAAAAIAAYagAAAAEGqi3ZYlron91xRQEi+Y8Plqp5Y9ig6r2OuoQnOicKvPwWBA972aOUZnzAyXDyEVUg==", "73457ee6-04f1-427e-86ba-6718c4a18c7e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "52bd15c4-13a9-467a-a415-3aae350805ef", "AQAAAAIAAYagAAAAEAjTgrsUtZb4f66o03uxwG5M3P4FlIyrOMGuNs/zc8VHsFmbVPk1rwXhpdmJJt87ww==", "602f8fd4-2b2d-46be-9088-f7fbf405c397" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "21d490a2-07d3-4bb0-8250-e30069a55e5f", "AQAAAAIAAYagAAAAEIVMbRKL9aisl1Ohw798KsHgble1D31K4n/SPJIST+M4/JPtUlElHjU5DNO2J74ufA==", "2a8868e9-c361-4934-9ba6-bfab9ab364e8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4aad0f79-459b-4ec7-9093-6b79d47779a8", "AQAAAAIAAYagAAAAEL6OrBwLD5qUn2KCbWKw3JoEqGSPDcl3xNfXpyNHYvlDb/VznMTL49Ax7HPtf8DDng==", "9843732c-76b8-44f9-82b3-ff9003620ba7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "64bffb7e-e45b-4f1e-9eb9-689fa3de313b", "AQAAAAIAAYagAAAAEPoM0iTK7i7MWGxQwXb60TL+yb2Y6+GJtwUHVEejOhw45v3GnxjZvQ8mVTKww82rnA==", "eae917d0-22c5-4ad3-b12f-0920f914f72e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d2a5b3c4-d7e8-4789-1234-56789abcdef3"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "77c12d82-2966-470d-ae8b-61b77e021eb6", "AQAAAAIAAYagAAAAEJTid92qx/wdlGgGHLFkytDBNtmmUXF+EaEPrFzKO/6LgLl6mT4kCYWxsKNLKu4reA==", "c189b960-fb27-4818-b80d-83783e05a45f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3b6c7d8-a9f0-4789-1234-56789abcdef4"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f471b7d-c104-475a-9202-86fd24dd8f76", "AQAAAAIAAYagAAAAEGGgVBpXlpal0y+UWPVovIQW1jd+m6nIOIIsAzveRY2u12s3wv7V8tMEgx005HFBdQ==", "73d0b1ef-2634-49e4-be0c-27a1f3188327" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("f4c7d8e9-b1a2-4789-1234-56789abcdef5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "455c47b6-bcf3-4c0e-9691-d56acec91a12", "AQAAAAIAAYagAAAAEGA0HNYY9ikYHXNb9W0HJoRundFp8oWakX0lXZBRzxUtZc0fpyqWXxSaHGSMUiQqEw==", "8a5f4525-ebbc-4585-9e7b-62a78cc4c191" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "TaskAttachments");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "TaskAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "TaskAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "TaskAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "TaskAttachments",
                type: "text",
                nullable: false,
                defaultValue: "");

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
        }
    }
}
