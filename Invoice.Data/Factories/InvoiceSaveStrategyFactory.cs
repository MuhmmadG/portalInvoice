using Invoice.Core.Interfaces;
using Invoice.Data.RopositoriesStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.Factories
{
    public class InvoiceSaveStrategyFactory  : IInvoiceSaveStrategyFactory
    { 
        private readonly Dictionary<string, IInvoiceSaveStrategy> _strategies;

        public InvoiceSaveStrategyFactory()
        {
            _strategies = new Dictionary<string, IInvoiceSaveStrategy>(StringComparer.OrdinalIgnoreCase)

        {
            { "I", new NormalInvoiceSaveStrategy() },
            { "C", new CreaditInvoiceStrategyOfPurshases() },

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
