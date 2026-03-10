using Invoice.Core.Interfaces;
using Invoice.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.Factories
{
    public static class InvoiceReaderFactory
    {
        public static IInvoiceReaderFile Create(string type)
        {
            return type switch
            {
                "xml" => new InvoiceReaderFile(),
              //  "json" => new JsonInvoiceReader(),
                _ => throw new NotSupportedException("نوع غير مدعوم")
            };
        }
    }
}
