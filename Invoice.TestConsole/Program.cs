using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Invoice.Data;              // مكان DbContext
using Invoice.Data.Data;
using Invoice.Data.Factories;
using Invoice.Data.RopositoriesStrategy;
using Invoice.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Models.Security;
using System;
namespace Invoice.TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //// 1️⃣ تجهز DbContextOptions
            //var options = new DbContextOptionsBuilder<AppDbContext>()
            //    .UseSqlServer("Server=192.168.1.21\\SQL25;Database=InvoicePortal;User Id=sa;Password=2408031;TrustServerCertificate=True;")
            //    .Options;
            //// 2️⃣ تعمل DbContext
            //using var dbContext = new AppDbContext(options);


            string folderPath = @"C:\Users\Muhmmad Gamal\Downloads\PurchacesInvoice\12-2025";

            var reader = InvoiceReaderFactory.Create("xml");
            var documents = reader.InvoiceReaderFiles(folderPath);


            //var entity = DocumentMapper.MapToEntity(documents, dbContext);

            //try
            //{
            //    dbContext.Documents.AddRangeAsync(entity);
            //    await dbContext.SaveChangesAsync();
            //}
            //catch (DbUpdateException ex)
            //{
            //    Console.WriteLine("DbUpdateException: " + ex.Message);
            //    if (ex.InnerException != null)
            //    {
            //        Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            //    }
            //    throw;
            //}


        }




    }
}

