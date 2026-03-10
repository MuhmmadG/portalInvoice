using ClosedXML.Excel;
using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Invoice.Data.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.RopositoriesStrategy
{
    public class BaseInvoiceStrategyOfSales : IInvoiceStrategy
    {
        private readonly string _filePath;
        private readonly IInvoiceSaveStrategyFactory _factory;
        public BaseInvoiceStrategyOfSales(string filePath , IInvoiceSaveStrategyFactory factory)
        {
            _filePath = filePath;
            _factory = factory;
        }
        public async Task SaveAsync(List<DocumentModelDto> documentModels)
        {
            if (string.IsNullOrWhiteSpace(_filePath))
                return;

            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using var workbook = new XLWorkbook(_filePath);

            var ws = workbook.Worksheets.FirstOrDefault() ?? workbook.AddWorksheet("مستندات المبيعات");
            int row = ws.LastRowUsed()?.RowNumber() + 1 ?? 1;

            var factory = new InvoiceSaveStrategyFactorySales();

            foreach (var doc in documentModels)
            {
                //if (doc.TaxTotals.All(t => t.TaxType == "T1" && t.Amount == 0)
                //    || doc.TaxTotals.All(t => t.TaxType != "T1"))
                //    continue;

                // البيانات الأساسية
              
                ws.Cell(row, 3).Value = 0;

                ws.Cell(row, 4).Value = doc.InternalId;
                ws.Cell(row, 5).Value = doc.ReceiverName;
                ws.Cell(row, 6).Value = doc.ReceiverId;
                ws.Cell(row, 8).Value = doc.Receiver.Address.Street;
                ws.Cell(row, 11).Value = doc.DateTimeReceived.ToString("dd.MM.yyyy");
                ws.Cell(row, 12).Value = "متنوع";

                ws.Cell(row, 17).Value = doc.TaxTotals.Where(t => t.TaxType == "T1").Sum(t => t.Amount) * 100 / 14;
                ws.Cell(row, 18).Value = 14;
                ws.Cell(row, 19).Value = 1;
                ws.Cell(row, 20).Value = doc.TaxTotals.Where(t => t.TaxType == "T1").Sum(t => t.Amount) * 100 / 14;
                ws.Cell(row, 22).Value = doc.TaxTotals.Where(t => t.TaxType == "T1").Sum(t => t.Amount) * 100 / 14;
                ws.Cell(row, 23).Value = doc.TaxTotals.Where(t => t.TaxType == "T1").Sum(t => t.Amount);
                ws.Cell(row, 24).Value = ws.Cell(row, 22).GetDouble() + ws.Cell(row, 23).GetDouble();

                // تطبيق الاستراتيجية حسب نوع الفاتورة
                var strategy = factory.GetStrategy(doc.TypeName);
                await strategy.SaveAsync(doc, ws, row);

                row++;
            }

            workbook.Save();
        }
    }
}
