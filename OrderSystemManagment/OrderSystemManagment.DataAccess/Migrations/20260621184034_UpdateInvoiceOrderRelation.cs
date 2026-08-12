using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderSystemManagment.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoiceOrderRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Invoices_InvoiceId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_InvoiceId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_InvoiceId",
                table: "Orders",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Invoices_InvoiceId",
                table: "Orders",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");
        }
    }
}
