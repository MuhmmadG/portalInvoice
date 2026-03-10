using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{
    public class ExternalExpense
    {
        public int? Id { get; set; }

        public DateTime Date { get; set; }
        public string? ReleaseNumber { get; set; }
        public decimal? Value { get; set; }
        public decimal? Vat { get; set; }
        public decimal? ProfitTax { get; set; }
        public decimal? ImportTax { get; set; }
        public decimal? Fees { get; set; }
        public decimal? Total { get; set; }
        public string? EPaymentNumber { get; set; }
        public string? Description { get; set; }

       
    }

}
