using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invoice.Data.Migrations
{
    /// <inheritdoc />
    public partial class DocumentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_ExpenseCategories_ExpenseCategoryId",
                table: "InvoiceLines");

            migrationBuilder.AddColumn<int>(
                name: "ExpenseCategoryId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ExpenseCategoryId",
                table: "Documents",
                column: "ExpenseCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ExpenseCategories_ExpenseCategoryId",
                table: "Documents",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_ExpenseCategories_ExpenseCategoryId",
                table: "InvoiceLines",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ExpenseCategories_ExpenseCategoryId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_ExpenseCategories_ExpenseCategoryId",
                table: "InvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ExpenseCategoryId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExpenseCategoryId",
                table: "Documents");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_ExpenseCategories_ExpenseCategoryId",
                table: "InvoiceLines",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
