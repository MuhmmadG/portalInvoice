using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Invoice.Data.Migrations
{
    /// <inheritdoc />
    public partial class Document : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportedItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InternalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PartyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    buildingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    governate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    regionCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    branchID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PartyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TypeVersionName = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DateTimeIssued = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeReceived = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalSales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssuerId = table.Column<int>(type: "int", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    ImportedItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_ImportedItems_ImportedItemId",
                        column: x => x.ImportedItemId,
                        principalTable: "ImportedItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Parties_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Parties_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    itemType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    itemCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    unitType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    unitValue_Id_PK = table.Column<int>(type: "int", nullable: false),
                    unitValue_CurrencySold = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    unitValue_AmountSold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    unitValue_AmountEGP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    unitValue_CurrencyExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    salesTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    netTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    totalTaxableFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    valueDifference = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DocumentModelId = table.Column<int>(type: "int", nullable: false),
                    ItemMappingId = table.Column<int>(type: "int", nullable: true),
                    ExpenseCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Documents_DocumentModelId",
                        column: x => x.DocumentModelId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_ExpenseCategories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_ItemMappings_ItemMappingId",
                        column: x => x.ItemMappingId,
                        principalTable: "ItemMappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaxTotals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DocumentModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxTotals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxTotals_Documents_DocumentModelId",
                        column: x => x.DocumentModelId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    InvoiceLineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxableItems_InvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "InvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "CategoryType" },
                values: new object[,]
                {
                    { 1, "مصروفات عمومية" },
                    { 2, "مصروفات تشغيلية" },
                    { 3, "رسوم" },
                    { 4, "تأمينات" },
                    { 5, "مشتريات" },
                    { 6, "م.خارجيه" },
                    { 7, "م.تسوقيه" },
                    { 8, "أخرى" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PartyId",
                table: "Addresses",
                column: "PartyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ImportedItemId",
                table: "Documents",
                column: "ImportedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IssuerId",
                table: "Documents",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ReceiverId",
                table: "Documents",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_DocumentModelId",
                table: "InvoiceLines",
                column: "DocumentModelId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_ExpenseCategoryId",
                table: "InvoiceLines",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_ItemMappingId",
                table: "InvoiceLines",
                column: "ItemMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMappings_TaxCode",
                table: "ItemMappings",
                column: "TaxCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxableItems_InvoiceLineId",
                table: "TaxableItems",
                column: "InvoiceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxTotals_DocumentModelId",
                table: "TaxTotals",
                column: "DocumentModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "TaxableItems");

            migrationBuilder.DropTable(
                name: "TaxTotals");

            migrationBuilder.DropTable(
                name: "InvoiceLines");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "ItemMappings");

            migrationBuilder.DropTable(
                name: "ImportedItems");

            migrationBuilder.DropTable(
                name: "Parties");
        }
    }
}
