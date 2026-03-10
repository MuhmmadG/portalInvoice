using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Model
{
    public class ExpenseAnalysisModel
    {
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public string InternalId { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
    }

}
