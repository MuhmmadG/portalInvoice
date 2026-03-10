
using ClosedXML.Graphics;
using Invoice.Core.Model;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Invoice.Data.Services
{
  
    public class InvoiceReportService
    {
        private readonly AppDbContext _context;

        public InvoiceReportService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<CustomerInvoiceDto>> GetInvoicesByDateRangeAsync(DateTime from, DateTime to)
        {
            // important: toInclusive يعني أول يوم بعد ToDate
            var toInclusive = to.Date.AddDays(1);
            // تحميل Party IDs (العملاء)
            var allowedPartyIds = await _context.Parties
                .Select(p => p.Id)
                .ToListAsync();

            var invoices = await _context.Documents
                  .Where(d => d.DateTimeReceived >= from.Date
                    && d.DateTimeReceived < toInclusive)
                .Where(d => allowedPartyIds.Contains((int)d.ReceiverId))
                .Include(d => d.Receiver)        // العميل
                .Include(d => d.TaxTotals)       // الضرائب
                .Include(d => d.InvoiceLines)    // البنود
                    .ThenInclude(l => l.ItemMapping) // الارتباط بالكود الداخلي
                .OrderBy(d => d.DateTimeReceived)
                .ToListAsync();

            return invoices.Select(d => new CustomerInvoiceDto
            {
                DateTimeReceived = d.DateTimeReceived,
                CustomerName = d.Receiver?.Name ?? "",
                InternalId = d.InternalId ?? "",

                // InternalCode (يأتي من ItemMapping داخل InvoiceLines)
                InternalCode = d.InvoiceLines.FirstOrDefault()?.ItemMapping?.InternalCode ?? "",

                NetAmount = d.NetAmount,
                Total = d.Total,

                // إجمالي الكمية
                quantity = d.InvoiceLines.Sum(l => l.quantity).ToString(),

                // الضرائب:
                TaxAmountT1 = d.TaxTotals.Where(t => t.TaxType == "T1")
        .Sum(t => t.Amount),

                TaxAmountT4 = d.TaxTotals.Where(t => t.TaxType == "T4")
                .Sum(t => t.Amount),

                
                // الضريبة T1 (القيمة المضافة)
                VatAmount = d.TaxTotals
        .Where(t => t.TaxType == "T1")
        .Sum(t => t.Amount),

                // ضريبة الأرباح التجارية T4
                ProfitTaxAmount = d.TaxTotals
        .Where(t => t.TaxType == "T4")
        .Sum(t => t.Amount),

                DocumentStatusType =
                    d.TypeVersionName.ToLower() == "i" ? "فاتوره" :
                    d.TypeVersionName.ToLower() == "c" ? "مرتجع" : ""
            })
            .ToList();
        }

        //public async Task<List<CustomerInvoiceDto>> GetInvoicesByDateRangeAsync(DateTime from, DateTime to)
        //{
        //    return await _context.Documents
        //        .Where(d => d.DateTimeReceived >= from && d.DateTimeReceived <= to)
        //        .Join(_context.Parties,
        //              d => d.ReceiverId,
        //              p => p.Id,
        //              (d, p) => new { d, p })
        //        .Join(_context.TaxTotals,
        //              dp => dp.d.Id,
        //              t => t.DocumentModelId,
        //              (dp, t) => new { dp.d, dp.p, t })
        //        .Join(_context.InvoiceLines,
        //              dpt => dpt.d.Id,
        //              i => i.DocumentModelId,

        //              (dpt, i) => new { dpt.d, dpt.p, dpt.t, i })
        //        .Join(_context.ItemMappings,
        //              dpti => dpti.i.ItemMappingId,
        //              it => it.Id,
        //              (dpti, it) => new { dpti.d, dpti.p, dpti.t, dpti.i, it })
        //        .OrderBy(x => x.d.DateTimeReceived)
        //        .Select(x => new CustomerInvoiceDto
        //        {
        //            DateTimeReceived = x.d.DateTimeReceived,
        //            CustomerName = x.p.Name,
        //            InternalId = x.d.InternalId,

        //            // ✔ InternalCode
        //            InternalCode = x.it.InternalCode,

        //            NetAmount = x.d.NetAmount,
        //            TaxAmount = x.t.Amount,
        //            Total = x.d.Total,
        //            quantity = x.i.quantity.ToString(),
        //            DocumentStatusType = x.d.TypeVersionName.ToLower() == "i" ? "مبيعات" :
        //                                 x.d.TypeVersionName.ToLower() == "c" ? "مرتجع" : ""
        //        })
        //        .ToListAsync();
        //}





    }

}
