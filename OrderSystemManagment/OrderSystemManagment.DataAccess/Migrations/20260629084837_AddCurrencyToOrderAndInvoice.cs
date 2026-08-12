using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderSystemManagment.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToOrderAndInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyTypeId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Orders SET CurrencyTypeId = COALESCE((SELECT TOP 1 Id FROM CurrencyTypes), 1)");

            migrationBuilder.Sql("UPDATE Invoices SET GrandTotal = '0'");

            migrationBuilder.AlterColumn<int>(
                name: "GrandTotal",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyTypeId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Invoices SET CurrencyTypeId = COALESCE((SELECT TOP 1 Id FROM CurrencyTypes), 1)");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "Invoices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ExchangeRateDate",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CurrencyTypeId",
                table: "Orders",
                column: "CurrencyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CurrencyTypeId",
                table: "Invoices",
                column: "CurrencyTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_CurrencyTypes_CurrencyTypeId",
                table: "Invoices",
                column: "CurrencyTypeId",
                principalTable: "CurrencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CurrencyTypes_CurrencyTypeId",
                table: "Orders",
                column: "CurrencyTypeId",
                principalTable: "CurrencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_CurrencyTypes_CurrencyTypeId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CurrencyTypes_CurrencyTypeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CurrencyTypeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CurrencyTypeId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CurrencyTypeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrencyTypeId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExchangeRateDate",
                table: "Invoices");

            migrationBuilder.AlterColumn<string>(
                name: "GrandTotal",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
