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
    public class NormalInvoiceSaveStrategySales : IInvoiceSaveStrategy
    {
        public Task SaveAsync(DocumentModelDto doc, IXLWorksheet ws, int row)
        {
            ws.Cell(row, 1).Value = 1;   // العمود الأول
            ws.Cell(row, 2).Value = 1;
            ws.Cell(row, 14).Value = 3;  // العمود 14
            ws.Cell(row, 15).Value = 1;  // العمود 15

            return Task.CompletedTask;
        }
    }
}
