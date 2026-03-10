using Invoice.Core.Model;
using Invoice.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Invoice.UI.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {
       
        public DashboardView()
        {
            InitializeComponent();
            var vm = new CustomerReportViewModel();
            DataContext = vm;

            Loaded += async (s, e) =>
            {
                await vm.LoadReportsAsync();
            };
           
        }
        private void Sales_Click(object sender, RoutedEventArgs e)
        {
            var invoiceView = new MainWindow();
            invoiceView.Show();
        }
        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            var purchaseView = new PurshaseView();
            purchaseView.Show();
        }
        private void ShowCustomerReport(object sender, RoutedEventArgs e)
        {
            var win = new CustomerReportWindow();
            win.Show();

        }
        //var showCustomerReportWindow = new CustomerReportWindow();
        //showCustomerReportWindow.Show();
        private void ShowProductReport(object sender, RoutedEventArgs e)
        {
            var showProductReportWindow = new ProductReportWindow();
            showProductReportWindow.Show();
        }
        private void ShowTaxReport_Click(object sender, RoutedEventArgs e)
        {
            var vm = AppHost.GetService<TaxReportViewModel>();
            var reportWindow = new TaxReportView { DataContext = vm };
            reportWindow.ShowDialog();
        }
        private void OpenCustomerJournal(object sender, RoutedEventArgs e)
        {
            var window = AppHost.GetService<CustomerJournalView>();
            window.ShowDialog();
        }
        private void ShowExternalExpense(object sender, RoutedEventArgs e)
        {
            var vm = AppHost.GetService<ExternalExpenseViewModel>(); // لو بتستخدم DI
            var view = new ExternalExpenseView
            {
                DataContext = vm
            };
            view.Show();
        }
        private void OtherExpense(object sender, RoutedEventArgs e)
        {
            var vm = AppHost.GetService<OtherExpenseViewModel>(); // لو بتستخدم DI
            var view = new OtherExpenseView
            {
                DataContext = vm
            };
            view.Show();
        }
        private void Transaction(object sender, RoutedEventArgs e)
        {
            var vm = AppHost.GetService<FinancialTransactionViewModel>(); // لو بتستخدم DI
            var view = new FinancialTransactionView
            {
                DataContext = vm
            };
            view.Show();
        }
        private void SupplierJournal(object sender, RoutedEventArgs e)
        {
            // استدعاء خدمة DI للحصول على النافذة View مع الـ ViewModel تلقائيًا
            var journalWindow = AppHost.GetService<SupplierJournalView>();

            // فتح النافذة
            journalWindow?.Show();
        }
        private void ExternalPurchaseReport(object sender, RoutedEventArgs e)
        {
            // استدعاء خدمة DI للحصول على النافذة View مع الـ ViewModel تلقائيًا
            var view = AppHost.GetService<ExternalPurchaseReportView>();
            view.Show();
        }
        private void ReportOthetExpanses(object sender, RoutedEventArgs e)
        {
            var view = AppHost.GetService<FinancialJournalView>();
            view.Show();
            view.Activate();
        }

        private void OpenExpenseAnalysis(object sender, RoutedEventArgs e)
        {

            var window = AppHost.GetService<ExpenseAnalysisView>();
            window.Show();

        }

        private void PriceTrends(object sender, RoutedEventArgs e)
        {
            if (DataContext is CustomerReportViewModel vm)
            {
                // تأكد أن Reports ليست فارغة
                if (vm.Reports == null || vm.Reports.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات لعرضها في المخططات.");
                    return;
                }

                // تمرير البيانات الصحيحة إلى DashboardView
                var dashboard = new CustomerDashboardView(vm.Reports.ToList());

                dashboard.Show();
            }
        }

        private void CustomerInvoices(object sender, RoutedEventArgs e)
        {
            var window = AppHost.GetService<CustomerInvoicesView>();
            window.Show();
        }

        private void SupplierInvoices(object sender, RoutedEventArgs e)
        {
            var window = AppHost.GetService<SupplierInvoicesView>();
            window.Show();
        }
        private void CustomerBalances(object sender, RoutedEventArgs e)
        {
            var window = AppHost.GetService<CustomerBalancesView>();
            window.Show();
        }
        private void SupplierBalancesView(object sender, RoutedEventArgs e)
        {
            var window = AppHost.GetService<SupplierBalancesView>();
            window.Show();
        }
    }
}
