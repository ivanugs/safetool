using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safetool.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralParameters",
                columns: table => new
                {
                    EmailAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAccountDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAccountPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAccountUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailPort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailServer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailSsl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralParameters");
        }
    }
}
