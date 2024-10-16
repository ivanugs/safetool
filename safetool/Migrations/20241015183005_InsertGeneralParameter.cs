using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safetool.Migrations
{
    /// <inheritdoc />
    public partial class InsertGeneralParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GeneralParameters",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneralParameters",
                table: "GeneralParameters",
                column: "Id");

            migrationBuilder.InsertData(
                table: "GeneralParameters",
                columns: new[] { "Id", "EmailAccount", "EmailAccountDisplayName", "EmailAccountPassword", "EmailAccountUser", "EmailPort", "EmailServer", "EmailSsl" },
                values: new object[] { 1, "mats.au_zu_fa@continental-corporation.com", "IT Management", "4pp54c0unt!!", "uig02796@contiwan.com", "2525", "SMTPHubEU.contiwan.com", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneralParameters",
                table: "GeneralParameters");

            migrationBuilder.DeleteData(
                table: "GeneralParameters",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GeneralParameters");
        }
    }
}
