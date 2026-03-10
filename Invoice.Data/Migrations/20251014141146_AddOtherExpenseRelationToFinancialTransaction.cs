using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invoice.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOtherExpenseRelationToFinancialTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OtherExpenseId",
                table: "FinancialTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_OtherExpenseId",
                table: "FinancialTransactions",
                column: "OtherExpenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialTransactions_OtherExpenses_OtherExpenseId",
                table: "FinancialTransactions",
                column: "OtherExpenseId",
                principalTable: "OtherExpenses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialTransactions_OtherExpenses_OtherExpenseId",
                table: "FinancialTransactions");

            migrationBuilder.DropIndex(
                name: "IX_FinancialTransactions_OtherExpenseId",
                table: "FinancialTransactions");

            migrationBuilder.DropColumn(
                name: "OtherExpenseId",
                table: "FinancialTransactions");
        }
    }
}
