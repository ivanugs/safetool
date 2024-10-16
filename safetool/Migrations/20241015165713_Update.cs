using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safetool.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeNumber",
                table: "FormSubmission");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeName",
                table: "FormSubmission",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeUID",
                table: "FormSubmission",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeUID",
                table: "FormSubmission");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeName",
                table: "FormSubmission",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeNumber",
                table: "FormSubmission",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);
        }
    }
}
