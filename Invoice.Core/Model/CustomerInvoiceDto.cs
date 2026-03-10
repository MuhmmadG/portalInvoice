using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{
    public class CustomerInvoiceDto
    {

        public DateTime DateTimeReceived { get; set; }
        public string Date => DateTimeReceived.ToString("yyyy-MM-dd");

        public string CustomerName { get; set; }
        public string InternalId { get; set; }
        public decimal NetAmount { get; set; }
        public decimal ProfitTaxAmount { get; set; }
        public decimal TaxAmountT1 { get; set; }
        public decimal TaxAmountT4 { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Total { get; set; }
        public string DocumentStatusType { get; set; }
        public string quantity { get; set; }

        public string InternalCode { get; set; }
    }

}
