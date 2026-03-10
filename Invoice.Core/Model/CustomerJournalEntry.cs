using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{
    // ✅ نموذج عرض اليومية (Customer)
    public class CustomerJournalEntry
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Amount { get; set; }
        public List<TaxDetail> TaxDetailsList { get; set; } = new();
        public decimal Balance { get; set; }

        // 🏦 إضافات جديدة
        public string? BankName { get; set; }
        public string? ChequeNumber { get; set; }
    }

}
