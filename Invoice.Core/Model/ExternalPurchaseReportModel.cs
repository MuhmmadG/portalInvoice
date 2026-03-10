using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{
    public class ExternalPurchaseReportModel
    {
        public DateTime ReportDate { get; set; }
        public string? InternalId { get; set; }
        public string? ItemDetails { get; set; }
        public string? ItemName { get; set; }
        public decimal? NumericValue { get; set; }
        public decimal TaxT1 { get; set; }
        public decimal TaxT4 { get; set; }
        public decimal? ImportTax { get; set; }
        public decimal? Fees { get; set; }
        public decimal? Total { get; set; }
        public string? EPaymentNumber { get; set; }
        public string? TaxNumber { get; set; }
        public string? Name { get; set; }
        public string? TypeVersionName { get; set; }
        public bool IsReturn => string.Equals(TypeVersionName, "c", StringComparison.OrdinalIgnoreCase);




    }

}
