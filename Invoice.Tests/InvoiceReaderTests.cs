using Invoice.Core.Model; // مكان الكلاس بتاعك  // مكان DTO
using Invoice.Data.RopositoriesStrategy;
using Invoice.Data.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

public class InvoiceReaderTests
{
    //[Fact]
    //public void InvoiceReaderFiles_ShouldExportToExcel()
    //{
    //    string folderPath = @"C:\Users\Muhmmad Gamal\Downloads\Purchases\8-2025";
    //    string excelPath = @"C:\Users\Muhmmad Gamal\Downloads\مستندات المشتريات_0.xlsx";
    //    var reader = new InvoiceReaderFile();
    //    var documents = reader.InvoiceReaderFiles(folderPath);
    //    var invoiceReader = new BaseInvoiceStrategyOfPurshases(folderPath);
    //    invoiceReader.SaveAsync(documents);

    //    Assert.True(File.Exists(excelPath));
    //}
}
