using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace safetool.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceType",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PPE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Risk",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risk", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RiskLevel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskLevel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Area_Location_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationID = table.Column<int>(type: "int", nullable: false),
                    AreaID = table.Column<int>(type: "int", nullable: false),
                    DeviceTypeID = table.Column<int>(type: "int", nullable: false),
                    RiskLevelID = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Function = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpecificFunction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Operators = table.Column<int>(type: "int", nullable: false),
                    LastMaintenance = table.Column<DateOnly>(type: "date", nullable: false),
                    EmergencyStopImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TypeSafetyDevice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FunctionSafetyDevice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Device_Area_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Area",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_DeviceType_DeviceTypeID",
                        column: x => x.DeviceTypeID,
                        principalTable: "DeviceType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_RiskLevel_RiskLevelID",
                        column: x => x.RiskLevelID,
                        principalTable: "RiskLevel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DevicePPE",
                columns: table => new
                {
                    DevicesID = table.Column<int>(type: "int", nullable: false),
                    PPEsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicePPE", x => new { x.DevicesID, x.PPEsID });
                    table.ForeignKey(
                        name: "FK_DevicePPE_Device_DevicesID",
                        column: x => x.DevicesID,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DevicePPE_PPE_PPEsID",
                        column: x => x.PPEsID,
                        principalTable: "PPE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceRisk",
                columns: table => new
                {
                    DevicesID = table.Column<int>(type: "int", nullable: false),
                    RisksID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceRisk", x => new { x.DevicesID, x.RisksID });
                    table.ForeignKey(
                        name: "FK_DeviceRisk_Device_DevicesID",
                        column: x => x.DevicesID,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceRisk_Risk_RisksID",
                        column: x => x.RisksID,
                        principalTable: "Risk",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DeviceType",
                columns: new[] { "ID", "Active", "Name" },
                values: new object[,]
                {
                    { 1, true, "Equipo de laboratorio" },
                    { 2, true, "Herramienta" }
                });

            migrationBuilder.InsertData(
                table: "RiskLevel",
                columns: new[] { "ID", "Active", "Level" },
                values: new object[,]
                {
                    { 1, true, "Alto" },
                    { 2, true, "Medio" },
                    { 3, true, "Bajo" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "ID", "Active", "Name" },
                values: new object[,]
                {
                    { 1, true, "Administrador" },
                    { 2, true, "Operador" }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "ID", "RoleID", "UserName" },
                values: new object[] { 1, 1, "uig65332" });

            migrationBuilder.CreateIndex(
                name: "IX_Area_LocationID",
                table: "Area",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Device_AreaID",
                table: "Device",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_Device_DeviceTypeID",
                table: "Device",
                column: "DeviceTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Device_RiskLevelID",
                table: "Device",
                column: "RiskLevelID");

            migrationBuilder.CreateIndex(
                name: "IX_DevicePPE_PPEsID",
                table: "DevicePPE",
                column: "PPEsID");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceRisk_RisksID",
                table: "DeviceRisk",
                column: "RisksID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleID",
                table: "UserRole",
                column: "RoleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevicePPE");

            migrationBuilder.DropTable(
                name: "DeviceRisk");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "PPE");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "Risk");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "DeviceType");

            migrationBuilder.DropTable(
                name: "RiskLevel");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
