using Invoice.Core.Model;
using Invoice.Data.Data;
using Invoice.UI.Services;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Invoice.UI.ViewModels
{
    public class CustomerJournalViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly PdfExportService _pdfExportService;

        public ObservableCollection<Party> Customers { get; set; } = new();
        public ObservableCollection<CustomerJournalEntry> JournalEntries { get; set; } = new();

        // 🟢 الخصائص الجديدة
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

        // اجمالي التحصيلات
        private decimal _totalReceipts;
        public decimal TotalReceipts
        {
            get => _totalReceipts;
            set { _totalReceipts = value; OnPropertyChanged(); }
        }

        private decimal _totalSales;
        public decimal TotalSales
        {
            get => _totalSales;
            set { _totalSales = value; OnPropertyChanged(); }
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


        private Party _selectedCustomer;
        public Party SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                if (_selectedCustomer != null)
                    LoadJournalEntries();
            }
        }

        public ICommand LoadJournalCommand { get; }
        public ICommand ExportPdfCommand { get; }

        public CustomerJournalViewModel(AppDbContext context, PdfExportService pdfExportService)
        {
            _context = context;
            _pdfExportService = pdfExportService;

            LoadCustomers();
            LoadJournalCommand = new RelayCommand(_ => LoadJournalEntries());
            ExportPdfCommand = new RelayCommand(_ => ExportToPdf());
        }

        private void ExportToPdf()
        {
            if (SelectedCustomer == null || !JournalEntries.Any())
            {
                MessageBox.Show("الرجاء اختيار عميل وتحميل اليومية أولاً.", "تنبيه",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var supplierJournalEntries = JournalEntries
                .Select(j => new SupplierJournalEntry
                {
                    Date = j.Date,
                    InvoiceNumber = j.InvoiceNumber,
                    Description = j.Description,
                    DebitAmount = j.DebitAmount,
                    CreditAmount = j.CreditAmount,
                    Amount = j.Amount,
                    TaxDetailsList = j.TaxDetailsList,
                    Balance = j.Balance
                });

            _pdfExportService.ExportSupplierJournal(SelectedCustomer.Name, supplierJournalEntries);
        }

        private void LoadCustomers()
        {
            try
            {
                var allCustomers = _context.Documents
                    .Include(d => d.Receiver)
                    .AsNoTracking()
                    .Where(d => d.Receiver != null)
                    .Select(d => d.Receiver!)
                    .ToList();

                var customers = allCustomers
                    .GroupBy(p => p.Name)
                    .Select(g => g.First())
                    .OrderBy(p => p.Name)
                    .ToList();

                Customers = new ObservableCollection<Party>(customers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل العملاء: {ex.Message}", "خطأ",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }  
        private void LoadJournalEntries()
        {
            JournalEntries.Clear();
            if (SelectedCustomer == null) return;

            try
            {
                // ✅ ضبط التاريخ الافتراضي: من أول الشهر حتى اليوم الحالي
                if (!StartDate.HasValue)
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                if (!EndDate.HasValue)
                    EndDate = DateTime.Now;

                string customerName = SelectedCustomer.Name;

                // ================================
                // 🧾 الفواتير (Documents)
                // ================================
                var invoicesQuery = _context.Documents
                    .AsNoTracking()
                    .Where(d => d.Receiver != null && d.Receiver.Name == customerName);

                // 🔹 فلترة بالتاريخ
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
                    bool isSaleReturn = inv.TypeVersionName != null &&
                                            inv.TypeVersionName.ToLower() == "c";
                    return new CombinedLedgerRow
                    {
                        TransactionDate = inv.DateTimeReceived,
                        DocumentReference = inv.InternalId,
                        TransactionType = isSaleReturn ? "مردودات" : "فاتورة ",
                        DebitAmount = isSaleReturn ? 0m : inv.NetAmount,
                        CreditAmount = isSaleReturn ? inv.Total :0m ,
                        NetChange = isSaleReturn ? -inv.Total : inv.Total,
                        SourceType = "Document",
                        SourceId = inv.Id,
                        TaxDetailsList = inv.Taxes
                        .Select(t => new TaxDetail { Rate = t.Rate, Amount = t.Amount })
                        .ToList(),
                        Amount = inv.Taxes.Sum(t => t.Amount)
                    };
                }).ToList();
                // ================================
                // 💵 التحصيلات (Receipts)
                // ================================
                var receiptsQuery = _context.FinancialTransactions
                    .AsNoTracking()
                    .Where(ft => ft.Party != null &&
                                 ft.Party.Name == customerName &&
                                 ft.TransactionType == TransactionType.Receipt);

                // 🔹 فلترة بالتاريخ
                if (StartDate.HasValue)
                    receiptsQuery = receiptsQuery.Where(ft => ft.Date >= StartDate.Value);
                if (EndDate.HasValue)
                    receiptsQuery = receiptsQuery.Where(ft => ft.Date <= EndDate.Value);

                var receiptsData = receiptsQuery.ToList(); // نحمل البيانات للذاكرة

                var receipts = receiptsData
                    .Select(ft => new CombinedLedgerRow
                    {
                        TransactionDate = ft.Date,
                        DocumentReference = string.Empty,
                        TransactionType = ft.PaymentMethod switch
                        {
                            PaymentMethod.Cash => "تحصيل / نقدي",
                            PaymentMethod.Cheque => "تحصيل / شيك",
                            PaymentMethod.BankTransfer => "تحصيل / تحويل بنكي",
                            _ => "تحصيل"
                        },
                        BankName = ft.BankName,
                        ChequeNumber = ft.ChequeNumber,
                        DebitAmount = null,
                        CreditAmount = ft.Amount,
                        NetChange = -ft.Amount,
                        SourceType = "FinancialTransaction",
                        SourceId = ft.Id,
                        Amount = 0m,
                        TaxDetailsList = new List<TaxDetail>()
                    })
                    .ToList();
                // ================================
                // 📊 دمج النتائج
                // ================================
                var combined = invoiceRows.Concat(receipts)
                    .OrderBy(x => x.TransactionDate)
                    .ThenBy(x => x.SourceType)
                    .ThenBy(x => x.SourceId)
                    .ToList();
                decimal runningBalance = 0m;
                foreach (var row in combined)
                {
                    runningBalance += row.NetChange;

                    JournalEntries.Add(new CustomerJournalEntry
                    {
                        Date = row.TransactionDate,
                        InvoiceNumber = row.SourceType == "Document" ? row.DocumentReference ?? string.Empty : string.Empty,
                        Description = row.TransactionType,
                        DebitAmount = row.DebitAmount ?? 0m,
                        CreditAmount = row.CreditAmount ?? 0m,
                        Amount = row.Amount ?? 0m,
                        TaxDetailsList = row.TaxDetailsList ?? new List<TaxDetail>(),
                        Balance = runningBalance,
                        BankName = row.BankName,
                        ChequeNumber = row.ChequeNumber
                    });
                }
                // ================================
                // 🧮 حساب الإجماليات (باستثناء المردودات)
                // ================================
                var nonReturnEntries = JournalEntries
                    .Where(e => e.Description != "مردودات")
                    .ToList();

                // ✅ إجمالي الفواتير (بدون المردودات)
                TotalInvoiceValue = nonReturnEntries.Sum(e => e.DebitAmount);

                // ✅ الضرائب
                TotalTax1 = nonReturnEntries
                    .SelectMany(e => e.TaxDetailsList)
                    .Where(t => t.Rate == 1)
                    .Sum(t => t.Amount);

                TotalTax14 = nonReturnEntries
                    .SelectMany(e => e.TaxDetailsList)
                    .Where(t => t.Rate == 14)
                    .Sum(t => t.Amount);

                // ✅ المبيعات الصافية
                TotalSales = TotalInvoiceValue + TotalTax14 - TotalTax1;

                // ✅ إجمالي التحصيلات (بدون المردودات طبعًا لأنها لا تُسجَّل تحصيل)
                TotalReceipts = JournalEntries
                    .Where(e => e.CreditAmount > 0 && e.Description != "مردودات")
                    .Sum(e => e.CreditAmount);

                // ✅ إجمالي المرتجعات
                TotalReturns = JournalEntries
                    .Where(e => e.Description == "مردودات")
                    .Sum(e => e.CreditAmount);

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
