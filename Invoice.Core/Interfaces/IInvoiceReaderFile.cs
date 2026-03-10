using Invoice.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Interfaces
{
    public interface IInvoiceReaderFile
    {
        List<DocumentModelDto> InvoiceReaderFiles(string folderPath);
    }
}
