using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Invoice.Core.Model;
using Invoice.Data.Data;
using Invoice.UI.Services;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
namespace Invoice.UI.ViewModels
{
    public class ExternalPurchaseReportViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly ExcelExportService _excelService;

        public ObservableCollection<ExternalPurchaseReportModel> ReportEntries { get; set; } = new();
        private decimal _totalTaxT1;
        public decimal TotalTaxT1
        {
            get => _totalTaxT1;
            set { _totalTaxT1 = value; OnPropertyChanged(); }
        }

        private decimal _totalTaxT4;
        public decimal TotalTaxT4
        {
            get => _totalTaxT4;
            set { _totalTaxT4 = value; OnPropertyChanged(); }
        }

        private decimal _totalPurchases;
        public decimal TotalPurchases
        {
            get => _totalPurchases;
            set { _totalPurchases = value; OnPropertyChanged(); }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }
        private ExternalPurchaseReportModel _selectedReport;
        public ExternalPurchaseReportModel SelectedReport
        {
            get => _selectedReport;
            set { _selectedReport = value; OnPropertyChanged(); }
        }


        // ✅ الخاصية الجديدة للـ ComboBox
        public ObservableCollection<string> ItemDescriptions { get; set; } = new();

        public ICommand LoadReportCommand { get; }
        public ICommand LoadDescriptionsCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        private string? _selectedDescription;
        public string? SelectedDescription
        {
            get => _selectedDescription;
            set
            {
                _selectedDescription = value;
                OnPropertyChanged();

                // عندما يختار المستخدم وصفًا من الـ ComboBox
                _ = LoadReportAsync();
            }
        }
        private async Task DeleteSelectedAsync()
        {
            if (SelectedReport == null)
            {
                MessageBox.Show("الرجاء تحديد الفاتورة المراد حذفها.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"هل أنت متأكد من حذف الفاتورة رقم {SelectedReport.InternalId}؟",
                "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                // نحاول الحذف من Documents أولاً
                var doc = await _context.Documents
                    .FirstOrDefaultAsync(d => d.InternalId == SelectedReport.InternalId);

                if (doc != null)
                {
                    _context.Documents.Remove(doc);
                }
                else
                {
                    // إذا لم تكن في Documents نحاول من ExternalExpenses
                    var exp = await _context.ExternalExpenses
                        .FirstOrDefaultAsync(e => e.ReleaseNumber == SelectedReport.ItemDetails);

                    if (exp != null)
                        _context.ExternalExpenses.Remove(exp);
                }

                await _context.SaveChangesAsync();

                ReportEntries.Remove(SelectedReport);
                SelectedReport = null;

                MessageBox.Show("تم حذف الفاتورة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ExternalPurchaseReportViewModel(AppDbContext context, ExcelExportService excelService)
        {
            _context = context;
            _excelService = excelService;
            LoadReportCommand = new RelayCommand(async _ => await LoadReportAsync());
            ExportToExcelCommand = new RelayCommand(_ => ExportToExcel());
            LoadDescriptionsCommand = new RelayCommand(async _ => await LoadDescriptionsAsync());
            DeleteSelectedCommand = new RelayCommand(async _ => await DeleteSelectedAsync(), _ => SelectedReport != null);
        }
        // دالة التصدير
        private void ExportToExcel()
        {
            _excelService.ExportExternalPurchaseReport(ReportEntries);
        }
        // ✅ تحميل القيم الفريدة للوصف
        public async Task LoadDescriptionsAsync()
        {
            var descriptions = await (
                from d in _context.Documents
                join i in _context.ImportedItems on d.ImportedItemId equals i.Id
                where i.Description != null
                select i.Description
            )
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

            ItemDescriptions.Clear();
            foreach (var desc in descriptions)
                ItemDescriptions.Add(desc);
        }
        public async Task LoadReportAsync()
        {
            // 🟩 استعلام المستندات
            var docs = await (
                from d in _context.Documents
                join i in _context.ImportedItems on d.ImportedItemId equals i.Id
                join p in _context.Parties on d.IssuerId equals p.Id
                where i.Description != null &&
                      (string.IsNullOrEmpty(SelectedDescription) || i.Description == SelectedDescription)
                   
                select new ExternalPurchaseReportModel
                {
                    Name = p.Name,
                    TaxNumber = p.TaxNumber,
                    ReportDate = d.DateTimeReceived,
                    InternalId = d.InternalId,
                    ItemDetails = i.Description,
                    ItemName = i.Name,
                    NumericValue = d.NetAmount,
                    TaxT1 = d.TaxTotals
                        .Where(t => t.TaxType == "T1")
                        .Sum(t => (decimal?)t.Amount) ?? 0,
                    TaxT4 = d.TaxTotals
                        .Where(t => t.TaxType == "T4")
                        .Sum(t => (decimal?)t.Amount) ?? 0,
                    ImportTax = null,
                    Fees = null,
                    Total = d.Total,
                    EPaymentNumber = null,
                    TypeVersionName = d.TypeVersionName
                }
            ).ToListAsync();

            // 🟩 استعلام المصروفات الخارجية
            var expenses = await (
                from e in _context.ExternalExpenses
                where e.ReleaseNumber != null &&
                      (string.IsNullOrEmpty(SelectedDescription) || e.ReleaseNumber == SelectedDescription)
                select new ExternalPurchaseReportModel
                {
                    Name = null,
                    TaxNumber = null,
                    ReportDate = e.Date,
                    InternalId = null,
                    ItemDetails = e.ReleaseNumber,
                    ItemName = e.Description,
                    NumericValue = e.Value,
                    TaxT4 = (decimal)e.Vat,
                    TaxT1 = (decimal)e.ProfitTax,
                    ImportTax = e.ImportTax,
                    Fees = e.Fees,
                    Total = e.Total,
                    EPaymentNumber = e.EPaymentNumber
                }
            ).ToListAsync();

            // 🟨 نجمع النتائج في الذاكرة (LINQ to Objects)
            var query = docs.Union(expenses)
                            .OrderBy(r => r.ReportDate)
                            .ToList();

            // 🟦 تعبئة DataGrid
            ReportEntries.Clear();
            foreach (var item in query)
                ReportEntries.Add(item);

            // 🟧 حساب الإجماليات
            TotalTaxT1 = ReportEntries.Sum(x => x.TaxT1);
            TotalTaxT4 = ReportEntries.Sum(x => x.TaxT4);
            TotalPurchases = ReportEntries.Sum(x => x.NumericValue ?? 0);
            TotalAmount = ReportEntries.Sum(x => x.Total ?? 0);
        }


    }

}
