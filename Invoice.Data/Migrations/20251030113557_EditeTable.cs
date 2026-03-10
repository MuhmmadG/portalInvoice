using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Invoice.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "CategoryType" },
                values: new object[,]
                {
                    { 9, "أصول" },
                    { 10, "صندوق الطوارئ" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.AddColumn<string>(
                name: "EPaymentNumber",
                table: "ExternalExpenses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Fees",
                table: "ExternalExpenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ImportTax",
                table: "ExternalExpenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ProfitTax",
                table: "ExternalExpenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "ExternalExpenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Vat",
                table: "ExternalExpenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
