using Invoice.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Interfaces
{
    public interface IInvoiceStrategy
    {
        Task SaveAsync(List<DocumentModelDto> documentModels);

    }
}
