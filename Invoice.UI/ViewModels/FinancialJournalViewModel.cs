using Invoice.Core.Model;
using Invoice.Data.Data;
using Invoice.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Invoice.UI.ViewModels
{
    public class FinancialJournalViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private readonly FinancialJournalPdfService _pdfService;
        public FinancialJournalViewModel(AppDbContext context, FinancialJournalPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
            JournalEntries = new ObservableCollection<FinancialJournalEntry>();
            ExpenseCategories = new ObservableCollection<ExpenseCategory>(_context.ExpenseCategories.AsNoTracking().ToList());

            LoadCommand = new RelayCommand(_ => LoadJournalEntries());
            PrintCommand = new RelayCommand(_ => PrintJournal());
            StartDate = DateTime.Today.AddMonths(-1);
            EndDate = DateTime.Today;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public ObservableCollection<ExpenseCategory> ExpenseCategories { get; set; }
        public ObservableCollection<FinancialJournalEntry> JournalEntries { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        private decimal _totalDebit;

        private decimal _totalBankCommission;
        public decimal TotalBankCommission
        {
            get => _totalBankCommission;
            set { _totalBankCommission = value; OnPropertyChanged(nameof(TotalBankCommission)); }
        }
        public decimal TotalDebit
        {
            get => _totalDebit;
            set { _totalDebit = value; OnPropertyChanged(nameof(TotalDebit)); }
        }

        private decimal _totalCredit;
        public decimal TotalCredit
        {
            get => _totalCredit;
            set { _totalCredit = value; OnPropertyChanged(nameof(TotalCredit)); }
        }

        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(nameof(Balance)); }
        }
        private ExpenseCategory _selectedExpenseCategory;
        public ExpenseCategory SelectedExpenseCategory
        {
            get => _selectedExpenseCategory;
            set
            {
                _selectedExpenseCategory = value;
                OnPropertyChanged(nameof(SelectedExpenseCategory));
                LoadJournalEntries(); // تحديث البيانات تلقائيًا عند تغيير النوع
            }
        }
        public ICommand LoadCommand { get; }
        public ICommand PrintCommand { get; }
        private void PrintJournal()
        {
            if (JournalEntries == null || !JournalEntries.Any())
            {
                MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _pdfService.ExportToPdf(JournalEntries, StartDate, EndDate, TotalDebit, TotalCredit, Balance);
        }

        private void LoadJournalEntries()
        {
            JournalEntries.Clear();

            try
            {
                // 🧾 تحميل المصروفات (Debit)
                var expenseEntries = _context.OtherExpenses
                    .Include(o => o.ExpenseCategory)
                    .Where(o => o.Date >= StartDate && o.Date <= EndDate);

                // ✅ تطبيق الفلترة لو تم اختيار نوع مصروف
                if (SelectedExpenseCategory != null)
                    expenseEntries = expenseEntries.Where(o => o.ExpenseCategoryId == SelectedExpenseCategory.Id);

                var debitList = expenseEntries.Select(o => new FinancialJournalEntry
                {
                    TransactionDate = o.Date,
                    Description = "تسجيل مصروف: " + o.Description,
                    Debit = o.Value,
                    Credit = null,
                    BankCommission = o.BankCommission
                });

                // 💰 تحميل السداد (Credit)
                var paymentEntries = _context.FinancialTransactions
                    .Include(f => f.OtherExpense)
                    .ThenInclude(oe => oe.ExpenseCategory)
                    .Where(f => f.Date >= StartDate && f.Date <= EndDate && f.OtherExpenseId != null);

                if (SelectedExpenseCategory != null)
                    paymentEntries = paymentEntries.Where(f => f.OtherExpense.ExpenseCategoryId == SelectedExpenseCategory.Id);

                var creditList = paymentEntries.Select(f => new FinancialJournalEntry
                {
                    TransactionDate = f.Date,
                    Description = "سداد مصروف (" + f.OtherExpense.Description + ")",
                    Debit = null,
                    Credit = f.Amount,
                    BankCommission = null
                });

                // 🔄 دمج القائمتين
                var combined = debitList.Concat(creditList)
                    .OrderBy(t => t.TransactionDate)
                    .ThenBy(t => t.Description)
                    .ToList();

                // 🧮 حساب الرصيد التراكمي
                decimal runningBalance = 0;
                foreach (var entry in combined)
                {
                    runningBalance += (entry.Debit ?? 0) + (entry.BankCommission ?? 0) - (entry.Credit ?? 0);
                    entry.Balance = runningBalance;
                    JournalEntries.Add(entry);
                }
                TotalBankCommission = combined.Sum(e => e.BankCommission ?? 0);
                TotalDebit = combined.Sum(e => e.Debit ?? 0);
                TotalCredit = combined.Sum(e => e.Credit ?? 0);
                Balance = (TotalDebit+ TotalBankCommission) - TotalCredit;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل اليومية: {ex.Message}",
                    "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

   
}
