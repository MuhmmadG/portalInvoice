using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{
    // ✅ كلاس مساعد لعرض النتائج
    public class FinancialJournalEntry
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = "";
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? BankCommission { get; set; }
        public decimal Balance { get; set; }
    }
}
