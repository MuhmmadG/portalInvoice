using ClosedXML.Excel;
using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.RopositoriesStrategy
{
    internal class CreaditInvoiceStrategyOfSales : IInvoiceSaveStrategy
    {
        public Task SaveAsync(DocumentModelDto doc, IXLWorksheet ws, int row)
        {
            ws.Cell(row, 1).Value = 3;   // العمود الأول
            ws.Cell(row, 2).Value = 1;   // العمود الثانى
            ws.Cell(row, 14).Value = 5;  // العمود 14
            ws.Cell(row, 15).Value = 1;  // العمود 15
            return Task.CompletedTask;
        }
    }
}
