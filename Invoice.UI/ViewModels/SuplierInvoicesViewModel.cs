using Invoice.Core.Model;
using Invoice.Data.Services;
using Invoice.UI.Services;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Invoice.UI.ViewModels
{
   public  class SuplierInvoicesViewModel : BaseViewModel
    {
        private int _correctInvoicesCount;
        public int CorrectInvoicesCount
        {
            get => _correctInvoicesCount;
            set => SetProperty(ref _correctInvoicesCount, value);
        }

        private int _returnedInvoicesCount;
        public int ReturnedInvoicesCount
        {
            get => _returnedInvoicesCount;
            set => SetProperty(ref _returnedInvoicesCount, value);
        }

        private decimal _totalVat;
        public decimal TotalVat
        {
            get => _totalVat;
            set => SetProperty(ref _totalVat, value);
        }

        private decimal _totalProfitTax;
        public decimal TotalProfitTax
        {
            get => _totalProfitTax;
            set => SetProperty(ref _totalProfitTax, value);
        }

        private decimal _totalSales;
        public decimal TotalSales
        {
            get => _totalSales;
            set => SetProperty(ref _totalSales, value);
        }

        private decimal _netResult;
        public decimal NetResult
        {
            get => _netResult;
            set => SetProperty(ref _netResult, value);
        }

        private readonly SupplierInvoiceReportService _reportService;

        public ObservableCollection<SupplierInvoiceDto> Invoices { get; set; }
            = new ObservableCollection<SupplierInvoiceDto>();

        private DateTime _fromDate = DateTime.Now.AddMonths(-1);
        public DateTime FromDate
        {
            get => _fromDate;
            set => SetProperty(ref _fromDate, value);
        }

        private DateTime _toDate = DateTime.Now;
        public DateTime ToDate
        {
            get => _toDate;
            set => SetProperty(ref _toDate, value);
        }

        public ICommand FilterCommand { get; }
        public ICommand ExportExcelCommand { get; }

        public SuplierInvoicesViewModel(SupplierInvoiceReportService reportService)
        {
            _reportService = reportService;
            FilterCommand = new RelayCommand(_ => LoadDataAsync());

            ExportExcelCommand = new RelayCommand(_ => ExportToExcel());
            _ = LoadDataAsync();
        }
        private void ExportToExcel()
        {
            if (Invoices == null || Invoices.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktop, $"SuplierInvoices-{DateTime.Now:yyyyMMddHHmm}.xlsx");

            OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Muhammed Gamal");

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Invoices");

                // عناوين الأعمدة
                ws.Cells["A1"].Value = "التاريخ";
                ws.Cells["B1"].Value = "المورد";
                ws.Cells["C1"].Value = "رقم الفاتورة";
                ws.Cells["D1"].Value = "تحليلى";
               
                ws.Cells["F1"].Value = "الصافي";
                ws.Cells["G1"].Value = "ض.ق.م";
                ws.Cells["H1"].Value = "ض.ا.ت";
                ws.Cells["I1"].Value = "الإجمالي";
                ws.Cells["J1"].Value = "نوع المستند";

                int row = 2;

                foreach (var item in Invoices)
                {
                    ws.Cells[row, 1].Value = item.DateTimeReceived.ToString("yyyy-MM-dd");
                    ws.Cells[row, 2].Value = item.SupplierName;
                    ws.Cells[row, 3].Value = item.InternalId;
                    ws.Cells[row, 4].Value = item.ExpenseCategory;
                    ws.Cells[row, 6].Value = item.NetAmount;
                    ws.Cells[row, 7].Value = item.TaxAmountT1;
                    ws.Cells[row, 8].Value = item.TaxAmountT4;
                    ws.Cells[row, 9].Value = item.Total;
                    ws.Cells[row, 10].Value = item.DocumentStatusType;

                    row++;
                }

                ws.Cells.AutoFitColumns();

                package.SaveAs(new FileInfo(filePath));
            }

            MessageBox.Show($"✔ تم حفظ الملف على سطح المكتب:\n{filePath}",
                            "تم بنجاح", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private async Task LoadDataAsync()
        {
            var data = await _reportService.GetSupplierInvoicesByDateRangeAsync(FromDate, ToDate);

            Invoices.Clear();
            foreach (var item in data)
                Invoices.Add(item);
            CalculateTotals();
        }
       
        private void CalculateTotals()
        {
            decimal sales = 0;
            decimal vat = 0;
            decimal profitTax = 0;

            CorrectInvoicesCount = Invoices.Count(x => x.DocumentStatusType != "مرتجع");
            ReturnedInvoicesCount = Invoices.Count(x => x.DocumentStatusType == "مرتجع");

            foreach (var inv in Invoices)
            {
                int sign = inv.DocumentStatusType == "مرتجع" ? -1 : 1;

                sales += sign * inv.NetAmount;
                vat += sign * inv.VatAmount;
                profitTax += sign * inv.ProfitTaxAmount;
            }

            TotalSales = sales;
            TotalVat = vat;
            TotalProfitTax = profitTax;

            NetResult = TotalSales + TotalVat - TotalProfitTax;
        }


    }
}
