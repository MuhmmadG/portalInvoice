 using DocumentFormat.OpenXml.Spreadsheet;
using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace Invoice.Data.RopositoriesStrategy
{
    internal class CreaditInvoiceStrategyOfPurshases : IInvoiceSaveStrategy
    {
        public Task SaveAsync(DocumentModelDto doc, IXLWorksheet ws, int row)
        {
            ws.Cell(row, 1).Value = 3;   // العمود الأول
            ws.Cell(row, 14).Value = 5;  // العمود 14
            ws.Cell(row, 15).Value = 3;  // العمود 15

            return Task.CompletedTask;
        }
    }
}
