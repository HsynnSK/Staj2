using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantNode.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionPlanEstimationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedEndDate",
                table: "ProductionPlans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TargetQuantity",
                table: "ProductionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedEndDate",
                table: "ProductionPlans");

            migrationBuilder.DropColumn(
                name: "TargetQuantity",
                table: "ProductionPlans");
        }
    }
}
