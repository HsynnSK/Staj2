using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOMManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBOMRouteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BOMLines_BOMRoutes_BOMRouteId",
                table: "BOMLines");

            migrationBuilder.DropTable(
                name: "BOMRoutes");

            migrationBuilder.DropIndex(
                name: "IX_BOMLines_BOMRouteId",
                table: "BOMLines");

            migrationBuilder.DropColumn(
                name: "BOMRouteId",
                table: "BOMLines");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BOMRouteId",
                table: "BOMLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BOMRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BOMHeaderId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OperationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OperationSeq = table.Column<int>(type: "int", nullable: false),
                    RunTime = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SetupTime = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkCenterCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOMRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BOMRoutes_BOMHeaders_BOMHeaderId",
                        column: x => x.BOMHeaderId,
                        principalTable: "BOMHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BOMLines_BOMRouteId",
                table: "BOMLines",
                column: "BOMRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_BOMRoutes_BOMHeaderId",
                table: "BOMRoutes",
                column: "BOMHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BOMLines_BOMRoutes_BOMRouteId",
                table: "BOMLines",
                column: "BOMRouteId",
                principalTable: "BOMRoutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
