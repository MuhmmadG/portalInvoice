using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{

    // ✅ صف التجميع الداخلي
    public class CombinedLedgerRow
    {
        public DateTime TransactionDate { get; set; }
        public string DocumentReference { get; set; }
        public string TransactionType { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? Credit { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal NetChange { get; set; }
        public string SourceType { get; set; }
        public int SourceId { get; set; }
        public string? BankName { get; set; }
        public string? ChequeNumber { get; set; }

        public decimal? Amount { get; set; }
        public List<TaxDetail> TaxDetailsList { get; set; } = new();
    }



}
