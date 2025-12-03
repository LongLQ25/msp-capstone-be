using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalInvitationSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "MemberId",
                table: "OrganizationInvitations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "InvitedEmail",
                table: "OrganizationInvitations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "OrganizationInvitations",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a5b6c7d8-e9f0-4789-1234-56789abcdef6"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6209cea5-24d4-43ed-bc02-206d586ebb71", "AQAAAAIAAYagAAAAEFyId01Z8KOcL4yRH2hedYaTurZvxGV6s0LSHparASLC8YaaNl3Vu8xHSbczuErlLA==", "15394275-451b-40a0-a5d8-331792d0ed01" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b6c7d8e9-f0a1-4789-1234-56789abcdef7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "323dc14b-cb60-4652-b7aa-5ce5a9740eca", "AQAAAAIAAYagAAAAEEzpSQE5w+/R6dUKA9vmWrmdIVg8Q9qprz8Nb+T4eBzwNwsPKJI2e0bUuaLG+l+ufw==", "d5d4809d-7079-48e8-97c4-726a63abe2f4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bae913d6-e850-4b5d-8f51-7cb4d4bddb8a", "AQAAAAIAAYagAAAAECTLmxDyE7Fi1PJficaZLAgLlrAtb85tlv4MFiYrPbGulYbB2XH0R38EyF7+IiG+FQ==", "da0904f8-df67-45c4-bdce-420d2693122d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a538ee82-5095-483b-950b-496ef3d788a8", "AQAAAAIAAYagAAAAEAqYiiN5iIspHuLOvGsO86VsBl9H4vBitu00F9T8kZ9r54qT6fBwQ6FKJ1NzJwam1A==", "e8924353-860d-4349-9a06-8bd1fa064f5c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6837227e-52ed-44c1-bb0c-8e823569ba65", "AQAAAAIAAYagAAAAEJ5Alc8229ZUPrEL5AiGNo8ac+D59hoG/CYJhgW8BlHlPsiVs1kH2gYIc6Zlg5RF/g==", "9f14f8af-5dac-4173-9848-f5c6d94beb65" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "10fddb3d-4618-4d0b-9364-634dacfff570", "AQAAAAIAAYagAAAAEA28XwK2dXsFhCst8cifSQlcRNZOjoc0OhFltucr3TuEq+k9NwyYXu9FRMMB9+W0nA==", "de48b501-a17d-4e33-88e7-25a9460c0495" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d2a5b3c4-d7e8-4789-1234-56789abcdef3"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aaf85cde-ddcb-4b9d-94b7-4be0393ae737", "AQAAAAIAAYagAAAAED8gVf52aJeunG66wDJUVO+t3v3mwsp8nJM8UzyjO4keuctWFQ6oHlbm5g0G/08gdg==", "137a363c-d3e3-47d8-822a-f290714eb138" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3b6c7d8-a9f0-4789-1234-56789abcdef4"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a081189a-45df-483b-bca7-dc09963483ea", "AQAAAAIAAYagAAAAEMIiPp35/J6VRh6+nMEyDtbHs9PnVhLiWNqmbwkEUNXHf3p2+ie04ry9CcIy0iEZrA==", "75cae239-ca0c-4d61-ada4-3f891595151a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("f4c7d8e9-b1a2-4789-1234-56789abcdef5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b3eaf52-72db-4597-bb1d-f445ae1df7c6", "AQAAAAIAAYagAAAAEGVgHExfenXIg5xOJceGtU25j270NzV8vgGdIm4e9/HtRhLlYIQlr+9JWcp7t/G9hQ==", "d08a6f9e-7212-477b-b0fe-c7c1bdca53fa" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvitations_Token",
                table: "OrganizationInvitations",
                column: "Token",
                unique: true,
                filter: "\"Token\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationInvitations_Token",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "InvitedEmail",
                table: "OrganizationInvitations");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "OrganizationInvitations");

            migrationBuilder.AlterColumn<Guid>(
                name: "MemberId",
                table: "OrganizationInvitations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a5b6c7d8-e9f0-4789-1234-56789abcdef6"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e5448f0d-bc3b-4f60-a0a3-bcf2e9832fdd", "AQAAAAIAAYagAAAAEP005ZuTIC16qtceg10vDZdwSkoTd32X//qKsh/f2hygePvTR7NnxpdwvNSuPd9fYQ==", "8708bb71-41b7-4345-a280-0923c59b6b4c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b6c7d8e9-f0a1-4789-1234-56789abcdef7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4678e87d-6a90-4b09-8dae-e4f4d7e49724", "AQAAAAIAAYagAAAAEFMaxYNS3uJs8tfXGo0HfqvGdJatOhtQXGbk8ji1g8zFHaCVMbpY7wXDcAODImBuow==", "7340adba-d5bd-4f6c-a232-2169305b1697" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d7a6cb8c-e94b-42c2-935f-d31c8493a3de", "AQAAAAIAAYagAAAAEPKnyt6bhNR8A58inxLWFbOanaxt0Th3m2mTnWP5CgEIWcTPRsAcE2X7P7Y3sOGaPA==", "374dc486-eb87-4768-948b-d48a3a3a01b5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c2d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7a16dde5-b887-406d-81a8-113fb681b5c1", "AQAAAAIAAYagAAAAEKKt3wNUJ/Z1yF6uhE73vfZeFqhjraYnSZOu+D+Wwcuf/YpRZe9I0O2sFeS1dc74KA==", "49f7b626-e8c2-4ac5-b263-e13e65ea09ae" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8bdcc577-f145-477c-9dc4-243252cd72e1", "AQAAAAIAAYagAAAAEBCxWLU9XkFeOLBmagFFu9E1PqCEl6ZFNm1dD6NWYz6iHUXZZajb3q7ZGx3SI7ZfaQ==", "d544d9a0-2926-4e18-b402-b6a8389e3dde" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c4d4e3f4-a5b6-4789-1234-56789abcdef2"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ee3f0bc3-90d9-4564-a4cc-7ba7364059ca", "AQAAAAIAAYagAAAAEKo7zS9TSgazWpC+St7OvivpGWUFvQLOQ4SjYtTOhAqg1yOH5b3KOKFU99epRuhuHg==", "5843cd13-3178-4f8d-905d-1c000dfbcdf8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d2a5b3c4-d7e8-4789-1234-56789abcdef3"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b59d16f2-8e3e-4536-9747-ab967017f553", "AQAAAAIAAYagAAAAENNho35Cm8xCiGW2A8VU5SScuxalV9gZBGzCwdLfii6jkEFXmb0YnltwwRdrJd/Nww==", "acd78a63-a1bf-4a6a-a9af-72373d8b371c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3b6c7d8-a9f0-4789-1234-56789abcdef4"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4316c206-ba5f-4a62-a8e5-2dc94188d704", "AQAAAAIAAYagAAAAENcGzvuPDu3rWkBv9dD1cFApn64oXUF/SIHieWdHBB8RVC+SDIT1H3wgD91Syx9m+Q==", "0e9f1ea3-707e-467c-8453-15136f104478" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("f4c7d8e9-b1a2-4789-1234-56789abcdef5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7e30f86e-4338-411f-873f-5af2c2449272", "AQAAAAIAAYagAAAAELPRFQZuutoAsUk83pwasnxkHg/y9YcBHu0pSw8SOpX4jWJspS2dDMAvAbrhwZDHJw==", "263822a5-8681-4238-a80a-862e5c8c8a0a" });
        }
    }
}
