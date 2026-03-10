using Invoice.Core.Model;
using Invoice.Data.Data;
using Invoice.UI.Services;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Invoice.UI.ViewModels
{
    public class SupplierJournalViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly JournalSuplierPDF _pdfExportService;

        public ObservableCollection<Party> Suppliers { get; set; } = new();
        public ObservableCollection<SupplierJournalEntry> JournalEntries { get; set; } = new();

        // 🟢 الخصائص الجديدة (الإجماليات)
        private decimal _totalInvoiceValue;
        public decimal TotalInvoiceValue
        {
            get => _totalInvoiceValue;
            set { _totalInvoiceValue = value; OnPropertyChanged(); }
        }

        private decimal _totalTax1;
        public decimal TotalTax1
        {
            get => _totalTax1;
            set { _totalTax1 = value; OnPropertyChanged(); }
        }

        private decimal _totalTax14;
        public decimal TotalTax14
        {
            get => _totalTax14;
            set { _totalTax14 = value; OnPropertyChanged(); }
        }

        private decimal _totalPayments;
        public decimal TotalPayments
        {
            get => _totalPayments;
            set { _totalPayments = value; OnPropertyChanged(); }
        }

        private decimal _totalPurchases;
        public decimal TotalPurchases
        {
            get => _totalPurchases;
            set { _totalPurchases = value; OnPropertyChanged(); }
        }
        private decimal _finalBalance;
        public decimal FinalBalance
        {
            get => _finalBalance;
            set { _finalBalance = value; OnPropertyChanged(); }
        }

        private decimal _totalReturns;
        public decimal TotalReturns
        {
            get => _totalReturns;
            set { _totalReturns = value; OnPropertyChanged(); }
        }


        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        private Party _selectedSupplier;
        public Party SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged();
                if (_selectedSupplier != null)
                    LoadJournalEntries();
            }
        }

        public ICommand LoadJournalCommand { get; }
        public ICommand ExportPdfCommand { get; }

        public SupplierJournalViewModel(AppDbContext context, JournalSuplierPDF pdfExportService)
        {
            _context = context;
            _pdfExportService = pdfExportService;

            LoadSuppliers();
            LoadJournalCommand = new RelayCommand(_ => LoadJournalEntries());
            ExportPdfCommand = new RelayCommand(_ => ExportToPdf());
        }

        private void ExportToPdf()
        {
            if (SelectedSupplier == null || !JournalEntries.Any())
            {
                MessageBox.Show("الرجاء اختيار مورد وتحميل اليومية أولاً.", "تنبيه",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _pdfExportService.ExportSupplierJournal(SelectedSupplier.Name, JournalEntries);
            
        }

        private void LoadSuppliers()
        {
            try
            {
                var allSuppliers = _context.Documents
                    .Include(d => d.Issuer)
                    .AsNoTracking()
                    .Where(d => d.Issuer != null)
                    .Select(d => d.Issuer!)
                    .ToList();

                var suppliers = allSuppliers
                    .GroupBy(p => p.Name)
                    .Select(g => g.First())
                    .OrderBy(p => p.Name)
                    .ToList();

                Suppliers = new ObservableCollection<Party>(suppliers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل الموردين: {ex.Message}", "خطأ",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadJournalEntries()
        {
            JournalEntries.Clear();
            if (SelectedSupplier == null) return;

            try
            {
                if (!StartDate.HasValue)
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                if (!EndDate.HasValue)
                    EndDate = DateTime.Now;

                string supplierName = SelectedSupplier.Name.ToLowerInvariant();

                // ================================
                // فواتير الموردين + مردودات المشتريات
                // ================================
                var invoicesQuery = _context.Documents
                    .AsNoTracking()
                    .Where(d => d.Issuer != null &&
                                d.Issuer.Name.ToLower() == supplierName);

                if (StartDate.HasValue)
                    invoicesQuery = invoicesQuery.Where(d => d.DateTimeReceived >= StartDate.Value);
                if (EndDate.HasValue)
                    invoicesQuery = invoicesQuery.Where(d => d.DateTimeReceived <= EndDate.Value);

                var invoices = invoicesQuery
                    .Select(d => new
                    {
                        d.DateTimeReceived,
                        d.InternalId,
                        d.NetAmount,
                        d.Total,
                        d.Id,
                        d.TypeVersionName,
                        Taxes = d.InvoiceLines
                            .SelectMany(il => il.TaxableItems)
                            .GroupBy(ti => ti.Rate)
                            .Select(g => new
                            {
                                Rate = g.Key,
                                Amount = g.Sum(ti => (decimal?)ti.Amount) ?? 0m
                            })
                            .ToList()
                    })
                    .ToList();

                var invoiceRows = invoices.Select(inv =>
                {
                    bool isPurchaseReturn = inv.TypeVersionName != null &&
                                            inv.TypeVersionName.ToLower() == "c";

                    return new CombinedLedgerRow
                    {
                        TransactionDate = inv.DateTimeReceived,
                        DocumentReference = inv.InternalId,
                        TransactionType = isPurchaseReturn ? "مردود مشتريات" : "فاتورة شراء",
                        // فاتورة شراء تزيد الالتزام => دائن (Credit)
                        Credit = isPurchaseReturn ? 0m : inv.NetAmount, 
                        DebitAmount = isPurchaseReturn ? inv.Total : 0m,   // المردود يقلل => مدين
                        CreditAmount = isPurchaseReturn ? 0m : inv.Total,  // الفاتورة دائن
                        NetChange = isPurchaseReturn ? -inv.Total : inv.Total,
                        SourceType = "Document",
                        SourceId = inv.Id,
                        TaxDetailsList = inv.Taxes
                            .Select(t => new TaxDetail { Rate = t.Rate, Amount = t.Amount })
                            .ToList(),
                        Amount = inv.Taxes.Sum(t => t.Amount)
                    };
                }).ToList();

                // ================================
                // المدفوعات (تُقلل الالتزام) -> تسجل كـ مدين (Debit)
                // ================================
                var paymentsQuery = _context.FinancialTransactions
                    .AsNoTracking()
                    .Where(ft => ft.Party != null &&
                                 ft.Party.Name.ToLower() == supplierName &&
                                 ft.TransactionType == TransactionType.Payment);

                if (StartDate.HasValue)
                    paymentsQuery = paymentsQuery.Where(ft => ft.Date >= StartDate.Value);
                if (EndDate.HasValue)
                    paymentsQuery = paymentsQuery.Where(ft => ft.Date <= EndDate.Value);

                var payments = paymentsQuery.ToList()
                    .Select(ft => new CombinedLedgerRow
                    {
                        TransactionDate = ft.Date,
                        DocumentReference = string.Empty,
                        TransactionType = ft.PaymentMethod switch
                        {
                            PaymentMethod.Cash => "سداد نقدي",
                            PaymentMethod.Cheque => "سداد شيك",
                            PaymentMethod.BankTransfer => "سداد تحويل بنكي",
                            _ => "سداد"
                        },
                        DebitAmount = ft.Amount,   // المدفوعات تقلل الالتزام => مدين
                        CreditAmount = 0m,
                        NetChange = -ft.Amount,
                        SourceType = "FinancialTransaction",
                        SourceId = ft.Id,
                        Amount = 0m,
                        TaxDetailsList = new List<TaxDetail>()
                    })
                    .ToList();

                // ================================
                // الدمج النهائي والترتيب
                // ================================
                var combined = invoiceRows.Concat(payments)
                    .OrderBy(x => x.TransactionDate)
                    .ThenBy(x => x.SourceType)
                    .ThenBy(x => x.SourceId)
                    .ToList();

                decimal runningBalance = 0m; // معناها: صافي دين المورد (Credit - Debit)
                foreach (var row in combined)
                {
                    // تحديث الرصيد: الرصيد يزيد بوجود دائن (التزام) وينقص بالمدين (سداد/مردود)
                    runningBalance += (row.CreditAmount ?? 0m) - (row.DebitAmount ?? 0m);

                    JournalEntries.Add(new SupplierJournalEntry
                    {
                        Date = row.TransactionDate,
                        InvoiceNumber = row.DocumentReference ?? string.Empty,
                        Description = row.TransactionType,
                        DebitAmount = row.DebitAmount ?? 0m,
                        CreditAmount = row.CreditAmount ?? 0m,
                        Credit = row.Credit ?? 0m,
                        Amount = row.Amount ?? 0m,
                        TaxDetailsList = row.TaxDetailsList ?? new List<TaxDetail>(),
                        Balance = runningBalance
                    });
                }



                // ================================
                // 🧮 حساب الإجماليات (باستثناء المردودات)
                // ================================
                var nonReturnEntries = JournalEntries
                    .Where(e => e.Description != "مردود مشتريات")
                    .ToList();

                // ✅ إجمالي الفواتير (فاتورة شراء فقط)
                TotalInvoiceValue = nonReturnEntries
                    .Where(e => e.CreditAmount > 0)
                    .Sum(e => e.CreditAmount);

                // ✅ إجمالي المدفوعات (استبعاد المردودات)
                TotalPayments = JournalEntries
                    .Where(e => e.DebitAmount > 0 && e.Description != "مردود مشتريات")
                    .Sum(e => e.DebitAmount);

                // ✅ ضرائب 1% و14% بدون المردودات
                TotalTax1 = nonReturnEntries
                    .SelectMany(e => e.TaxDetailsList)
                    .Where(t => t.Rate == 1)
                    .Sum(t => t.Amount);

                TotalTax14 = nonReturnEntries
                    .SelectMany(e => e.TaxDetailsList)
                    .Where(t => t.Rate == 14)
                    .Sum(t => t.Amount);

                // ✅ إجمالي المشتريات (فاتورة شراء فقط)
                TotalPurchases = nonReturnEntries
                    .Where(e => e.Credit > 0)
                    .Sum(e => e.Credit);

                // ✅ إجمالي المرتجعات
                TotalReturns = JournalEntries
                    .Where(e => e.Description == "مردود مشتريات")
                    .Sum(e => e.DebitAmount);

                // ✅ الرصيد النهائي (آخر صف في اليومية)
                FinalBalance = JournalEntries.Any() ? JournalEntries.Last().Balance : 0m;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل اليومية: {ex.Message}", "خطأ",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }

}
