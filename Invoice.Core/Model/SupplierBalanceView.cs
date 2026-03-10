using System;
using System.Collections.Generic;
using System.Text;

namespace Invoice.Core.Model
{
    public class SupplierBalanceView
    {
        public string SupplierName { get; set; }

        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal NetBalance { get; set; }

        public string BalanceStatus { get; set; }
    }
}
