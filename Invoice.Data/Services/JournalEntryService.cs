using Invoice.Core.Model;
using Invoice.Data.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Invoice.Data.Services
{
    public class JournalEntryService
    {
        private readonly AppDbContext _context;

        public JournalEntryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddEntryAsync(JournalEntry entry)
        {
            // ✅ تحقق من التوازن
            var totalDebit = entry.Lines.Sum(l => l.Debit);
            var totalCredit = entry.Lines.Sum(l => l.Credit);

            if (totalDebit != totalCredit)
                throw new Exception("القيد غير متوازن");

            // ✅ تحقق من الحسابات (Leaf فقط)
            foreach (var line in entry.Lines)
            {
                var acc = await _context.ChartOfAccounts.FindAsync(line.AccountId);

                if (acc == null)
                    throw new Exception("حساب غير موجود");

                if (!acc.IsPosting)
                    throw new Exception("لا يمكن القيد على حساب رئيسي");
            }

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
        }
    }


}
