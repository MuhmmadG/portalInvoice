using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{
    public class SupplierInvoiceDto
    {
        public DateTime DateTimeReceived { get; set; }
        public string Date => DateTimeReceived.ToString("yyyy-MM-dd");

        public string SupplierName { get; set; } = "";
        public string InternalId { get; set; } = "";
        public decimal NetAmount { get; set; }
        public decimal TaxAmountT1 { get; set; }
        public decimal TaxAmountT4 { get; set; }
        public decimal Total { get; set; }
        public string DocumentStatusType { get; set; } = "";
        public string quantity { get; set; } = "0";
        public string ExpenseCategory { get; set; } = "";
        public string InternalCode { get; set; } = "";
        public decimal VatAmount { get; set; }           // T1
        public decimal ProfitTaxAmount { get; set; }     // T4

    }

}
