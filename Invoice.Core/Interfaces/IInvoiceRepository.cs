using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Invoice.Core.Model;
namespace Invoice.Core.Interfaces
{
    public interface IInvoiceRepository
    {
        Task SaveInvoiceAsync(Model.DocumentModel documentModel);
    }
}
