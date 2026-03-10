using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invoice.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseFinancialFields3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EPaymentNumber",
                table: "ExternalExpenses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Fees",
                table: "ExternalExpenses",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ImportTax",
                table: "ExternalExpenses",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ProfitTax",
                table: "ExternalExpenses",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "ExternalExpenses",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Vat",
                table: "ExternalExpenses",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EPaymentNumber",
                table: "ExternalExpenses");

            migrationBuilder.DropColumn(
                name: "Fees",
                table: "ExternalExpenses");

            migrationBuilder.DropColumn(
                name: "ImportTax",
                table: "ExternalExpenses");

            migrationBuilder.DropColumn(
                name: "ProfitTax",
                table: "ExternalExpenses");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "ExternalExpenses");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "ExternalExpenses");
        }
    }
}
