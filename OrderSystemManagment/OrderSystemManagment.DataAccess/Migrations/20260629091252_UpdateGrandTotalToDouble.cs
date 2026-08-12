using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderSystemManagment.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGrandTotalToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "GrandTotal",
                table: "Invoices",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GrandTotal",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
