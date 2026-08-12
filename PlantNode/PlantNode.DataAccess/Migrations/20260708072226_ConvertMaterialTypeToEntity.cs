using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantNode.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ConvertMaterialTypeToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaterialType",
                table: "Products",
                newName: "MaterialTypeId");

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaxMachineSpeed = table.Column<float>(type: "real", nullable: false),
                    TensionLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccelerationRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_MaterialTypeId",
                table: "Products",
                column: "MaterialTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MaterialTypes_MaterialTypeId",
                table: "Products",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MaterialTypes_MaterialTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "MaterialTypes");

            migrationBuilder.DropIndex(
                name: "IX_Products_MaterialTypeId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "MaterialTypeId",
                table: "Products",
                newName: "MaterialType");
        }
    }
}
