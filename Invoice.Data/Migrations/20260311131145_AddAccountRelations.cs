using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invoice.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "TaxTotals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "FinancialTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChartOfAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FinancialStatement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    ParentAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartOfAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartOfAccounts_ChartOfAccounts_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxTotals_AccountId",
                table: "TaxTotals",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_AccountId",
                table: "FinancialTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_CodeAccount",
                table: "ChartOfAccounts",
                column: "CodeAccount",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_ParentAccountId",
                table: "ChartOfAccounts",
                column: "ParentAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialTransactions_ChartOfAccounts_AccountId",
                table: "FinancialTransactions",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxTotals_ChartOfAccounts_AccountId",
                table: "TaxTotals",
                column: "AccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialTransactions_ChartOfAccounts_AccountId",
                table: "FinancialTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxTotals_ChartOfAccounts_AccountId",
                table: "TaxTotals");

            migrationBuilder.DropTable(
                name: "ChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_TaxTotals_AccountId",
                table: "TaxTotals");

            migrationBuilder.DropIndex(
                name: "IX_FinancialTransactions_AccountId",
                table: "FinancialTransactions");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "TaxTotals");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "FinancialTransactions");
        }
    }
}
