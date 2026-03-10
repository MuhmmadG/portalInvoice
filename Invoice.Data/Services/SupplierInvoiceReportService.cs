using Invoice.Core.Model;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.Services
{
    public class SupplierInvoiceReportService
    {
        

        private readonly AppDbContext _context;
        public SupplierInvoiceReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierInvoiceDto>> GetSupplierInvoicesByDateRangeAsync(DateTime from, DateTime to)
        {
            // important: toInclusive يعني أول يوم بعد ToDate
            var toInclusive = to.Date.AddDays(1);
            var allowedPartyIds = await _context.Parties
       .Select(p => p.Id)
       .ToListAsync();             
            var invoices = await _context.Documents

                  .Where(d => d.DateTimeReceived >= from.Date
                    && d.DateTimeReceived < toInclusive)
                 .Where(d => allowedPartyIds.Contains((int)d.IssuerId))
                .Include(d => d.Issuer)
                .Include(d => d.TaxTotals)
                .Include(d => d.InvoiceLines)
                .Include(d => d.ExpenseCategory)
                .OrderBy(d => d.DateTimeReceived)
                .ToListAsync();

            return invoices.Select(d => new SupplierInvoiceDto
            {
                DateTimeReceived = d.DateTimeReceived,

                SupplierName = d.Issuer?.Name ?? "",
                InternalId = d.InternalId ?? "",
                ExpenseCategory = d.ExpenseCategory?.CategoryType ?? "",
                NetAmount = d.NetAmount,

                TaxAmountT1 = d.TaxTotals.Where(t => t.TaxType == "T1")
        .Sum(t => t.Amount),

                TaxAmountT4 = d.TaxTotals.Where(t => t.TaxType == "T4")
                .Sum(t => t.Amount),
                Total = d.Total,

                // الضريبة T1 (القيمة المضافة)
                VatAmount = d.TaxTotals
        .Where(t => t.TaxType == "T1")
        .Sum(t => t.Amount),

                // ضريبة الأرباح التجارية T4
                ProfitTaxAmount = d.TaxTotals
        .Where(t => t.TaxType == "T4")
        .Sum(t => t.Amount),

                quantity = d.InvoiceLines.Sum(l => l.quantity).ToString(),

                DocumentStatusType =
                    d.TypeVersionName.ToLower() == "i" ? "فاتوره" :
                    d.TypeVersionName.ToLower() == "c" ? "مرتجع" : "",
            }).ToList();
        }

     
    }
}
