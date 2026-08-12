using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantNode.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTypeToCoilWindingMachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "Machines",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "Machines");
        }
    }
}
