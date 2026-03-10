using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Invoice.Data.Data;
using Invoice.Data.Factories;
using Invoice.Data.RopositoriesStrategy;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Invoice.UI.ViewModels
{
    public class PurchaseViewModel : INotifyPropertyChanged
    {
       

        #region Fields

        private readonly AppDbContext _dbContext;
        private readonly IInvoiceSaveStrategyFactory _factory;
        private readonly DocumentMapper _mapper;
        private readonly IInvoiceReaderFile reader = InvoiceReaderFactory.Create("xml");
        private IInvoiceStrategy strategy;

        private ObservableCollection<DocumentModelDto> _invoices;
        private string _folderPath;
        private string _excelPath;
        //private DocumentModel _selectedInvoice;
        
        #endregion

        #region Properties

        public ObservableCollection<DocumentModelDto> Invoices
        {
            get => _invoices;
            set
            {
                _invoices = value;
                OnPropertyChanged(nameof(Invoices));
            }
        }

        public string FolderPath
        {
            get => _folderPath;
            set
            {
                _folderPath = value;
                OnPropertyChanged(nameof(FolderPath));
                // إعادة تقييم حالة الأوامر
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string ExcelPath
        {
            get => _excelPath;
            set
            {
                _excelPath = value;
                OnPropertyChanged(nameof(ExcelPath));
            }
        }

      
        #endregion

        #region Summary Properties

        private int _invoiceCount;
        public int InvoiceCount
        {
            get => _invoiceCount;
            set { _invoiceCount = value; OnPropertyChanged(nameof(InvoiceCount)); }
        }

        private decimal _salesTotal;
        public decimal SalesTotal
        {
            get => _salesTotal;
            set { _salesTotal = value; OnPropertyChanged(nameof(SalesTotal)); }
        }

        private decimal _vatTotal;
        public decimal VatTotal
        {
            get => _vatTotal;
            set { _vatTotal = value; OnPropertyChanged(nameof(VatTotal)); }
        }

        private decimal _tax1Total;
        public decimal Tax1Total
        {
            get => _tax1Total;
            set { _tax1Total = value; OnPropertyChanged(nameof(Tax1Total)); }
        }
      
        private decimal _grandTotal;
        public decimal GrandTotal
        {
            get => _grandTotal;
            set { _grandTotal = value; OnPropertyChanged(nameof(GrandTotal)); }
        }
        private ObservableCollection<ExpenseCategory> _expenseCategories;
        public ObservableCollection<ExpenseCategory> ExpenseCategories
        {
            get => _expenseCategories;
            set { _expenseCategories = value; OnPropertyChanged(nameof(ExpenseCategories)); }
        }



        private string _importedItemName;
        public string ImportedItemName
        {
            get => _importedItemName;
            set { _importedItemName = value; OnPropertyChanged(nameof(ImportedItemName)); }
        }

       
        #endregion

        #region Commands

        public ICommand SaveInvoiceCommand { get; }
        public ICommand SaveInvoiceCommandPurshases { get; }
        public ICommand SelectFolderCommand { get; private set; }
        public ICommand ShowInvoicesCommand { get; }
        public ICommand SaveToDatabaseCommand { get; }

        #endregion

        #region Constructor

        public PurchaseViewModel(IInvoiceSaveStrategyFactory factory, AppDbContext dbContext, DocumentMapper mapper)
        {
            _factory = factory;

            SaveInvoiceCommandPurshases = new RelayCommand(
                async _ => await SaveInvoiceAsyncPurshases(),
                _ => !string.IsNullOrWhiteSpace(FolderPath)
            );

            SelectFolderCommand = new RelayCommand(_ => SelectFolder());

            ShowInvoicesCommand = new RelayCommand(
                _ => LoadInvoices(),
                _ => !string.IsNullOrWhiteSpace(FolderPath)
            );
           
            _dbContext = dbContext;
            _mapper = mapper;
            ExpenseCategories = new ObservableCollection<ExpenseCategory>(_dbContext.ExpenseCategories.ToList());
            SaveToDatabaseCommand = new RelayCommand(
                async _ => await SaveToDatabaseAsync(),
                _ => Invoices != null && Invoices.Count > 0
            );
        }

        #endregion

        #region Methods - Save Invoices
        // ✅ زر مستقل للحفظ في Excel
        private async Task SaveInvoiceAsyncPurshases()
        {
            SelectExcelFile();
            var documents = reader.InvoiceReaderFiles(FolderPath);

            strategy = InvoiceFactory.CreateInvoiceStrategy(
                ExcelPath,
                InvoiceType.Purchases,
                _factory
            );

            await strategy.SaveAsync(documents);
        }

        // ✅ زر مستقل للحفظ في قاعدة البيانات
        private async Task SaveToDatabaseAsync()
        {
            if (Invoices == null || Invoices.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد فواتير للحفظ.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 🔹 التحقق من الفواتير التي لا تحتوي على مصروف
            var missingExpense = Invoices
                .Where(i => i.ExpenseCategoryId == null)
                .Select(i => i.InternalId)
                .ToList();

            if (missingExpense.Any())
            {
                string message = "الرجاء اختيار نوع المصروف للفواتير التالية:\n" +
                                 string.Join(", ", missingExpense);

                MessageBox.Show(message, "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // 🚫 لا يتم الحفظ إذا لم يتم اختيار المصروف لكل الفواتير
            }

            try
            {
                // 🔹 جميع الفواتير بها مصروف => يتم الحفظ
                foreach (var doc in Invoices)
                {
                    await _mapper.MapAndSaveDocumentPurchaese(
                        new List<DocumentModelDto> { doc },
                        doc.ExpenseCategoryId.Value);
                }

                MessageBox.Show("✅ تم حفظ جميع الفواتير بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الحفظ:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Methods - Load & Select

        public void LoadInvoices()
        {
            var docs = reader.InvoiceReaderFiles(FolderPath);

            // ✅ ترتيب حسب التاريخ تصاعدي
            var sortedDocs = docs
                .OrderBy(i => i.DateTimeReceived)
                .ToList();

            Invoices = new ObservableCollection<DocumentModelDto>(sortedDocs);

            // تحديث الملخص
            InvoiceCount = Invoices.Count;
            SalesTotal = Invoices.Sum(i => i.TotalAmount);

            // ضرائب (هنا انت كاتب TaxType = "T1" و "T4" للتجميع)
            VatTotal = Invoices.Sum(i => i.InvoiceLines.Sum(l =>
                l.TaxableItems.Where(t => t.TaxType == "T1").Sum(t => t.Amount)));

            Tax1Total = Invoices.Sum(i => i.InvoiceLines.Sum(l =>
                l.TaxableItems.Where(t => t.TaxType == "T4").Sum(t => t.Amount)));

            GrandTotal = Invoices.Sum(i => i.TotalSales);
            //الخصم
            

        }


        private void SelectFolder()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                FolderPath = dialog.SelectedPath;
            }
        }

        private void SelectExcelFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls"
            };
            if (dialog.ShowDialog() == true)
            {
                ExcelPath = dialog.FileName;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
