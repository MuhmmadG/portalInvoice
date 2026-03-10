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
    public class ExportInvoiceStrategy : IInvoiceSaveStrategy
    {
        public Task SaveAsync(DocumentModelDto doc, IXLWorksheet ws, int row)
        {
            ws.Cell(row, 1).Value = 8;   // العمود الأول
            ws.Cell(row, 2).Value = 1;   // العمود الثانى
            ws.Cell(row, 14).Value = 3;  // العمود 14
            ws.Cell(row, 15).Value = 2;  // العمود 15
            ws.Cell(row, 18).Value = 0;
            ws.Cell(row, 17).Value = doc.Total;
            ws.Cell(row, 20).Value = doc.Total;
            ws.Cell(row, 22).Value = doc.Total;
            ws.Cell(row, 23).Value = 0;
            ws.Cell(row, 24).Value = doc.Total;

            return Task.CompletedTask;
        }
    }
}
