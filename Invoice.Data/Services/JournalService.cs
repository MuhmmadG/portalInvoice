using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Invoice.Data.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Invoice.Data.Services
{
    public class JournalService : IJournalService
    {
        private readonly AppDbContext _context;

        public JournalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddEntryAsync(JournalEntry entry)
        {
            if (entry.Lines == null || !entry.Lines.Any())
                throw new Exception("القيد لا يحتوي على بنود");

            var totalDebit = entry.Lines.Sum(x => x.Debit);
            var totalCredit = entry.Lines.Sum(x => x.Credit);

            if (totalDebit != totalCredit)
                throw new Exception("القيد غير متوازن");

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
        }
        public async Task CreatePurchaseEntryAsync(DocumentModel doc, int debitAccountId, int creditAccountId)
        {
            var entry = new JournalEntry
            {
                Date = doc.DateTimeReceived,
                Description = $"فاتورة مشتريات رقم {doc.InternalId}",
                DocumentModelId = doc.Id,
                Lines = new List<JournalEntryLine>
        {
            new JournalEntryLine
            {
                AccountId = debitAccountId,
                Debit = doc.Total
            },
            new JournalEntryLine
            {
                AccountId = creditAccountId,
                Credit = doc.Total
            }
        }
            };

            await AddEntryAsync(entry);
        }
    
}
}
