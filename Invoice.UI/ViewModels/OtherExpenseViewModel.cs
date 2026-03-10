using Invoice.Core.Model;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
namespace Invoice.UI.ViewModels
{
    public class OtherExpenseViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public ObservableCollection<OtherExpense> Expenses { get; set; } = new();
        public ObservableCollection<ExpenseCategory> ExpenseCategories { get; set; } = new();

        private OtherExpense _selectedExpense;
        public OtherExpense SelectedExpense
        {
            get => _selectedExpense;
            set
            {
                _selectedExpense = value;
                OnPropertyChanged();
            }
        }

        private OtherExpense _currentExpense = new(); // 🔹 هذا هو كائن الإدخال
        public OtherExpense CurrentExpense
        {
            get => _currentExpense;
            set
            {
                _currentExpense = value;
                OnPropertyChanged();
            }
        }

        public ExpenseCategory SelectedCategory { get; set; }

        // الخصائص مربوطة بـ CurrentExpense فقط
        public DateTime Date
        {
            get => CurrentExpense.Date;
            set { CurrentExpense.Date = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => CurrentExpense.Description;
            set { CurrentExpense.Description = value; OnPropertyChanged(); }
        }

        public decimal Value
        {
            get => CurrentExpense.Value;
            set { CurrentExpense.Value = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
        }

        public decimal BankCommission
        {
            get => CurrentExpense.BankCommission;
            set { CurrentExpense.BankCommission = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
        }

        public decimal Total => Value + BankCommission;

        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public OtherExpenseViewModel(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            AddCommand = new RelayCommand(_ => AddExpense());
            SaveCommand = new RelayCommand(async _ => await SaveExpensesAsync());
            DeleteCommand = new RelayCommand(_ => DeleteSelected());

            LoadExpenseCategories();
            LoadExpenses();

            Date = DateTime.Today;
        }

        private void LoadExpenseCategories()
        {
            var data = _context.ExpenseCategories.AsNoTracking().ToList();
            ExpenseCategories = new ObservableCollection<ExpenseCategory>(data);
        }

        private void AddExpense()
        {
            if (SelectedCategory == null)
                return;

            var expense = new OtherExpense
            {
                Date = Date,
                Description = Description,
                Value = Value,
                BankCommission = BankCommission,
                ExpenseCategoryId = SelectedCategory.Id,
                ExpenseCategory = null
            };

            Expenses.Add(expense);
            ClearFields();
        }

        private async Task SaveExpensesAsync()
        {
            foreach (var e in Expenses)
            {
                if (e.Id == 0)
                {
                    e.ExpenseCategory = null;
                    _context.OtherExpenses.Add(e);
                }
            }

            await _context.SaveChangesAsync();

            // ✅ عرض رسالة نجاح
            System.Windows.MessageBox.Show("تم حفظ البيانات بنجاح ✅",
                                           "نجاح العملية",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Information);
        }


        private void DeleteSelected()
        {
            if (SelectedExpense == null)
                return;

            var result = MessageBox.Show(
                "هل تريد بالتأكيد حذف هذا المصروف؟",
                "تأكيد الحذف",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var expenseToDelete = SelectedExpense;

            // 🔴 احذف المعاملات المالية المرتبطة
            var transactions = _context.FinancialTransactions
                .Where(x => x.OtherExpenseId == expenseToDelete.Id)
                .ToList();

            if (transactions.Any())
            {
                _context.FinancialTransactions.RemoveRange(transactions);
            }

            // 🔴 احذف المصروف نفسه
            var entity = _context.OtherExpenses.Find(expenseToDelete.Id);
            if (entity != null)
            {
                _context.OtherExpenses.Remove(entity);
            }

            _context.SaveChanges();

            Expenses.Remove(expenseToDelete);

            SelectedExpense = null;
            OnPropertyChanged(nameof(SelectedExpense));

            MessageBox.Show("تم حذف المصروف بنجاح 🗑",
                            "نجاح العملية",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }



        private void LoadExpenses()
        {
            var data = _context.OtherExpenses
                .Include(o => o.ExpenseCategory)
                .ToList();

            Expenses = new ObservableCollection<OtherExpense>(data);
        }
        private void ClearFields()
        {
            Date = DateTime.Today;
            Description = string.Empty;
            Value = 0;
            BankCommission = 0;
        }
    }


}
