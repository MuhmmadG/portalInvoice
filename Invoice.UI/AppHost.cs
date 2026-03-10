using Invoice.Core.Interfaces;
using Invoice.Core.Interfaces.Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Invoice.Data.Data;
using Invoice.Data.Factories;
using Invoice.Data.RopositoriesStrategy;
using Invoice.Data.Services;
using Invoice.UI.Services;
using Invoice.UI.ViewModels;
using Invoice.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Invoice.UI
{
    public static class AppHost
    {
        private static ServiceProvider _serviceProvider;

        public static void ConfigureServices()
        {
            var services = new ServiceCollection();

            // 1️⃣ تسجيل DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(AppConfiguration.GetConnectionString()));

            // 2️⃣ تسجيل Factory
            services.AddScoped<IInvoiceSaveStrategyFactory, InvoiceSaveStrategyFactory>();

            // 3️⃣ تسجيل Services إضافية
            services.AddSingleton<INotificationService, MessageBoxNotificationService>();

            // 4️⃣ تسجيل ViewModels
            services.AddTransient<InvoiceViewModel>();

            services.AddTransient<PurchaseViewModel>();
            services.AddTransient<ExternalExpenseViewModel>();

            services.AddTransient<OtherExpenseViewModel>();
            services.AddTransient<TaxReportViewModel>();
            services.AddTransient<FinancialTransactionViewModel>();

            services.AddTransient<SupplierJournalViewModel>();
            services.AddTransient<SupplierJournalView>();


            services.AddTransient<CustomerJournalViewModel>();
            services.AddTransient<CustomerJournalView>();


            services.AddTransient<ExternalPurchaseReportViewModel>();
            services.AddTransient<ExternalPurchaseReportView>();

            services.AddScoped<FinancialJournalPdfService>();
            services.AddTransient<PdfExportService>();
            services.AddTransient<JournalSuplierPDF>();
            services.AddTransient<ExcelExportService>();


            services.AddTransient<DashboardView>();
           

            services.AddScoped<DocumentMapper>();

            services.AddScoped<FinancialJournalViewModel>();
            services.AddScoped<FinancialJournalView>();
         
            services.AddScoped<ExpenseAnalysisViewModel>();

            services.AddScoped<ExpenseAnalysisView>();

            

            //services.AddScoped<CustomerReportWindow>();
            services.AddScoped<CustomerReportViewModel>();

            services.AddScoped<CustomerDashboardView>();
            services.AddScoped<CustomerDashboardViewModel>();


            services.AddScoped<InvoiceReportService>();

            services.AddTransient<CustomerInvoicesViewModel>();
            services.AddTransient<CustomerInvoicesView>();

            services.AddScoped<SupplierInvoiceReportService>(); // ⬅️ مفقودة

            services.AddTransient<SuplierInvoicesViewModel>();
            services.AddTransient<SupplierInvoicesView>();

            services.AddTransient<CustomerBalancesViewModel>();
            services.AddTransient<CustomerBalancesView>();

            services.AddTransient<SupplierBalancesViewModel>();
            services.AddTransient<SupplierBalancesView>();



            _serviceProvider = services.BuildServiceProvider();
        }

        public static T GetService<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
 
}
