using Invoice.Core.Interfaces;
using Invoice.Data.Data;
using Invoice.Data.Factories;
using Invoice.UI.ViewModels;
using Invoice.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models;
using QuestPDF.Infrastructure;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
namespace Invoice.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Muhammed Gamal");
            AppHost.ConfigureServices();

            QuestPDF.Settings.License = LicenseType.Community;

            //var mainWindow = new MainWindow
            //{
            //    DataContext = AppHost.GetService<InvoiceViewModel>()
            //};
            var dashboard = AppHost.GetService<DashboardView>();
            dashboard.Show();

            // mainWindow.Show();
           

        }


    }

}
