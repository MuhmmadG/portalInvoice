using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{
    public class ProductReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string InternalCode { get; set; }
        public string Kind { get; set; }
        public decimal Price { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
