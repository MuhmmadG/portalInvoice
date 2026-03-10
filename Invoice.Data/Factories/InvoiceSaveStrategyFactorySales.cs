using Invoice.Core.Interfaces;
using Invoice.Data.RopositoriesStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.Factories
{
    public class InvoiceSaveStrategyFactorySales : IInvoiceSaveStrategyFactory
    {
        private readonly Dictionary<string, IInvoiceSaveStrategy> _strategies;
        public InvoiceSaveStrategyFactorySales()
        {
            _strategies = new Dictionary<string, IInvoiceSaveStrategy>(StringComparer.OrdinalIgnoreCase)

         {
            { "I", new NormalInvoiceSaveStrategySales() },
            { "C", new CreaditInvoiceStrategyOfSales() },
            { "ei", new ExportInvoiceStrategy() },
           };
        }

     

        public IInvoiceSaveStrategy GetStrategy(string typeName)
        {
            if (_strategies.TryGetValue(typeName, out var strategy))
            {
                return strategy;
            }
            throw new NotSupportedException($"No strategy found for typeName {typeName}");
        }
    }

}
