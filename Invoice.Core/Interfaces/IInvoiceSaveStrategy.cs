using ClosedXML.Excel;
using Invoice.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Interfaces
{
    public interface IInvoiceSaveStrategy
    {
        Task SaveAsync(DocumentModelDto doc, IXLWorksheet ws, int row);
       


    }

}
