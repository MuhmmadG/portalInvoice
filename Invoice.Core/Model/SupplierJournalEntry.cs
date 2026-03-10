using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{

    // ✅ الكيان المعروض في الجدول
    public class SupplierJournalEntry
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Credit { get; set; }

        public decimal Amount { get; set; }
        public List<TaxDetail> TaxDetailsList { get; set; } = new();
        public decimal Balance { get; set; }
    }

    public class TaxDetail
    {
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }






}
