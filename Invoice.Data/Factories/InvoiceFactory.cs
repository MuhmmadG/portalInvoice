using Invoice.Core.Interfaces;
using Invoice.Data.RopositoriesStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.Factories
{
    public enum InvoiceType
    {
        Sales,
        Purchases
    }
    public class InvoiceFactory
    {

        public static IInvoiceStrategy CreateInvoiceStrategy(string excelPath , InvoiceType type, IInvoiceSaveStrategyFactory factory)
        {
            return type switch
            {
                InvoiceType.Sales => new BaseInvoiceStrategyOfSales(excelPath, factory),
                InvoiceType.Purchases => new BaseInvoiceStrategyOfPurshases(excelPath, factory),
                _ => throw new NotSupportedException($"Invoice type {type} not supported")
            };
        }

    }
}
