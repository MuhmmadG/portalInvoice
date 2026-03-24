using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Vml;
using Invoice.Core.Model;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Invoice.Data.RopositoriesStrategy
{
    public class DocumentMapper
    {

        private readonly AppDbContext _context;

        public DocumentMapper(AppDbContext context)
        {
            _context = context;
        }
        public async Task<DocumentModel> MapAndSaveDocumentPurchaese(List<DocumentModelDto> dto, int selectedExpenseCategoryId)
        {
            DocumentModel savedDoc = null;
            foreach (var item in dto)
            {
                var entity = MapDocumentPurchase(item, selectedExpenseCategoryId);

                AttachImportedItem(item, entity);

                _context.Documents.Add(entity);
                savedDoc = entity;
            }

            await _context.SaveChangesAsync();
            return savedDoc;
        }
        public async Task MapAndSaveDocumentSale(List<DocumentModelDto> dto)
        {
            foreach (var item in dto)
            {
                var entity = MapDocumentSale(item);
              
                _context.Documents.Add(entity);
            }
            await _context.SaveChangesAsync();
        }

        private Party MapPartyIssuer(DocumentModelDto item)
        {
            return new Party
            {
                Name = item.IssuerName ?? string.Empty,
                TaxNumber = item.IssuerId ?? string.Empty,
                PartyType = item.Issuer?.Type ?? string.Empty,

                Address = new Address
                {
                    branchID = item.Issuer?.Address?.BranchID ?? string.Empty,
                    buildingNumber = item.Issuer?.Address?.BuildingNumber ?? string.Empty,
                    street = (item.Issuer?.Address?.Street?.Length > 100
                        ? item.Issuer.Address.Street.Substring(0, 100)
                        : item.Issuer?.Address?.Street) ?? string.Empty,
                    regionCity = item.Issuer?.Address?.RegionCity ?? string.Empty,
                    country = item.Issuer?.Address?.Country ?? string.Empty,
                    governate = item.Issuer?.Address?.Governate ?? string.Empty
                }
            };
        }
        private Party  MapPartyReceiver(DocumentModelDto item)
        {
            return new Party
            {
                Name = item.ReceiverName ?? string.Empty,
                TaxNumber = item.ReceiverId ?? string.Empty,
                PartyType = item.Receiver?.Type ?? string.Empty,
                Address = new Address
                {
                    branchID = item.Receiver?.Address?.BranchID ?? string.Empty,
                    buildingNumber = item.Receiver?.Address?.BuildingNumber ?? string.Empty,
                    street = (item.Receiver?.Address?.Street?.Length > 100
                        ? item.Receiver.Address.Street.Substring(0, 100)
                        : item.Receiver?.Address?.Street) ?? string.Empty,
                    regionCity = item.Receiver?.Address?.RegionCity ?? string.Empty,
                    country = item.Receiver?.Address?.Country ?? string.Empty,
                    governate = item.Receiver?.Address?.Governate ?? string.Empty
                }
            };
        }
        private DocumentModel MapDocumentPurchase(DocumentModelDto item, int expenseCategoryId)
        {
            var doc = new DocumentModel
            {
                InternalId = item.InternalId,
                DateTimeIssued = item.DateTimeIssued,
                ExtraDiscountAmount = item.ExtraDiscountAmount,
                DateTimeReceived = item.DateTimeReceived,
                TotalSales = item.TotalSales,
                NetAmount = item.NetAmount,
                Total = item.Total,
                Status = item.Status,
                TypeVersionName = item.TypeName,
                ExpenseCategoryId = expenseCategoryId,
                Issuer = MapPartyIssuer(item),
                InvoiceLines = MapInvoiceLinesPurchase(item),
                TaxTotals = MapTaxTotals(item)
            };

            // إذا كان المستخدم قد اختر حساب مدين ودائن من الواجهة، نسجل قيد مزدوج (Debit & Credit)
            // Debit -> SelectedAccountId (مثلاً: مصروفات مشتريات)
            // Credit -> SelectedCreditAccountId (مثلاً: موردين)
            if (item.SelectedAccountId.HasValue && item.SelectedCreditAccountId.HasValue)
            {
                // مدين
                var debit = new FinancialTransaction
                {
                    Date = item.DateTimeReceived,
                    TransactionType = TransactionType.Payment,
                    AccountId = item.SelectedAccountId.Value,
                    Amount = item.Total,
                    Notes = $"مدين - فاتورة مشتريات {item.InternalId}"
                };

                // دائن
                var credit = new FinancialTransaction
                {
                    Date = item.DateTimeReceived,
                    TransactionType = TransactionType.Payment,
                    AccountId = item.SelectedCreditAccountId.Value,
                    Amount = item.Total * -1, // سالب لتمثيل الجانب الدائن
                    Notes = $"دائن - فاتورة مشتريات {item.InternalId}"
                };

                //doc.FinancialTransactions.Add(debit);
                //doc.FinancialTransactions.Add(credit);
            }

            return doc;
        }
        private DocumentModel MapDocumentSale(DocumentModelDto item)
        {
            return new DocumentModel
            {
                InternalId = item.InternalId,
                DateTimeIssued = item.DateTimeIssued,
                DateTimeReceived = item.DateTimeReceived,
                TotalSales = item.TotalSales,
                NetAmount = item.NetAmount,
                ExtraDiscountAmount = item.ExtraDiscountAmount,
                Total = item.Total,
                Status = item.Status,
                TypeVersionName = item.TypeName,
                Receiver = MapPartyReceiver(item),
                InvoiceLines = MapInvoiceLinesSale(item),
                TaxTotals = MapTaxTotals(item)
            };
        }

        private List<TaxTotal> MapTaxTotals(DocumentModelDto item)
        {
            return item.TaxTotals?.Select(t => new TaxTotal
            {
                TaxType = t.TaxType,
                Amount = t.Amount
            }).ToList();
        }

        private List<InvoiceLine> MapInvoiceLinesPurchase(DocumentModelDto item)
        {
            return item.InvoiceLines?.Select(line => new InvoiceLine
            {
                description = line.Description,
                itemType = line.ItemType,
                itemCode = line.ItemCode,
                unitType = line.UnitType,
                quantity = line.Quantity,
                salesTotal = line.SalesTotal,
                netTotal = line.NetTotal,
                totalTaxableFees = line.TotalTaxableFees,
                valueDifference = line.ValueDifference,
                total = line.Total,
                unitValue = new UnitValue
                {
                    CurrencySold = line.UnitValue?.CurrencySold,
                    AmountSold = line.UnitValue?.AmountSold ?? 0,
                    AmountEGP = line.UnitValue?.AmountEGP ?? 0,
                    CurrencyExchangeRate = line.UnitValue?.CurrencyExchangeRate ?? 0
                },
                TaxableItems = line.TaxableItems?.Select(t => new TaxableItem
                {
                    TaxType = t.TaxType,
                    Amount = t.Amount,
                    SubType = t.SubType,
                    Rate = t.Rate
                }).ToList()
            }).ToList();
        }
        private List<InvoiceLine> MapInvoiceLinesSale(DocumentModelDto item)
        {
            return item.InvoiceLines?.Select(line =>
            {
                var mapping = GetOrCreateMapping(_context, line.ItemCode);
                return new InvoiceLine
                {
                    description = line.Description,
                    itemType = line.ItemType,
                    itemCode = mapping?.InternalCode ?? line.ItemCode,
                    unitType = line.UnitType,
                    quantity = line.Quantity,
                    salesTotal = line.SalesTotal,
                    netTotal = line.NetTotal,
                    totalTaxableFees = line.TotalTaxableFees,
                    valueDifference = line.ValueDifference,
                    total = line.Total,
                    unitValue = new UnitValue
                    {
                        CurrencySold = line.UnitValue?.CurrencySold,
                        AmountSold = line.UnitValue?.AmountSold ?? 0,
                        AmountEGP = line.UnitValue?.AmountEGP ?? 0,
                        CurrencyExchangeRate = line.UnitValue?.CurrencyExchangeRate ?? 0
                    },
                    TaxableItems = line.TaxableItems?.Select(t => new TaxableItem
                    {
                        TaxType = t.TaxType,
                        Amount = t.Amount,
                        SubType = t.SubType,
                        Rate = t.Rate
                    }).ToList(),
                    ItemMapping = mapping
                };
            }).ToList();
        }
        private void AttachImportedItem(DocumentModelDto item, DocumentModel entity)
        {
            var firstLineDesc = item.InvoiceLines?.FirstOrDefault()?.Description;

            if (!string.IsNullOrEmpty(firstLineDesc))
            {
                var importedItem = new ImportedItem
                {
                    Name = firstLineDesc,
                    Description = item.ImportedItem?.Description
                };

                _context.Add(importedItem);
                entity.ImportedItem = importedItem;
            }
        }

        public static ItemCodeConfig GetOrCreateMapping(
         AppDbContext context,
         string taxCode,
         string jsonPath = @"ItemCodes.json")
        {
            if (string.IsNullOrEmpty(taxCode))
                return null;

            // ابحث في قاعدة البيانات
            var mapping = context.ItemCodeConfig
                .FirstOrDefault(m => m.TaxCode == taxCode);

            if (mapping == null)
            {
                // حمّل المابنج من JSON
                var configs = Load(jsonPath);
                var config = configs.FirstOrDefault(c => c.TaxCode == taxCode);

                mapping = new ItemCodeConfig
                {
                    TaxCode = taxCode,
                    InternalCode = config?.InternalCode ?? taxCode,
                    Kind = config?.Kind ?? "غير محدد"
                };

                context.ItemCodeConfig.Add(mapping);
                context.SaveChanges(); // بيتسجل مرة واحدة بس في جدول المابنج
            }

            return mapping; // بيرجع الكائن (قديم أو جديد)
        }

        public static List<ItemCodeConfig> Load(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<ItemCodeConfig>();

            var json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<ItemCodeConfig>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ItemCodeConfig>();
        }
    }
}





