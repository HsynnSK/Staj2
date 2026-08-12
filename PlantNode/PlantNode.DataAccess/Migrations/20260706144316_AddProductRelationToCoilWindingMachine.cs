using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantNode.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductRelationToCoilWindingMachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductType",
                table: "Machines",
                newName: "CurrentProductId");

            migrationBuilder.Sql("UPDATE Machines SET CurrentProductId = NULL WHERE CurrentProductId IS NOT NULL");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaterialType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Machines_CurrentProductId",
                table: "Machines",
                column: "CurrentProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Machines_Products_CurrentProductId",
                table: "Machines",
                column: "CurrentProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machines_Products_CurrentProductId",
                table: "Machines");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Machines_CurrentProductId",
                table: "Machines");

            migrationBuilder.RenameColumn(
                name: "CurrentProductId",
                table: "Machines",
                newName: "ProductType");
        }
    }
}
