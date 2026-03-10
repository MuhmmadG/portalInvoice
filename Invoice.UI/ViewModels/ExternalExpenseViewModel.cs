using Invoice.Core.Model;
using Invoice.Data.Data;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Invoice.UI.ViewModels
{
    public class ExternalExpenseViewModel : BaseViewModel
    {

        private readonly AppDbContext _context;

        public ObservableCollection<ExternalExpense> Expenses { get; set; }

        private ExternalExpense _selectedExpense;
        public ExternalExpense SelectedExpense
        {
            get => _selectedExpense;
            set
            {
                _selectedExpense = value;
                OnPropertyChanged(nameof(SelectedExpense));
            }
        }

        // خصائص الإدخال
        public DateTime Date { get; set; } = DateTime.Now;
        public string ReleaseNumber { get; set; }
        public decimal Value { get; set; }
        public decimal Vat { get; set; }
        public decimal ProfitTax { get; set; }
        public decimal ImportTax { get; set; }
        public decimal Fees { get; set; }
        public decimal Total { get; set; }
        public string EPaymentNumber { get; set; }
        public string Description { get; set; }

        public ExternalExpenseViewModel(AppDbContext context)
        {
            _context = context;
            Expenses = new ObservableCollection<ExternalExpense>(_context.ExternalExpenses.ToList());
        }

        // ✅ دالة الإضافة
        public void AddExpense()
        {
            var expense = new ExternalExpense
            {
                Date = Date,
                ReleaseNumber = ReleaseNumber,
                Value = Value,
                Vat = Vat,
                ProfitTax = ProfitTax,
                ImportTax = ImportTax,
                Fees = Fees,
                Total = Total,
                EPaymentNumber = EPaymentNumber,
                Description = Description
            };

            _context.ExternalExpenses.Add(expense);
            _context.SaveChanges();

            Expenses.Add(expense);
            // 🟢 بعد الحفظ امسح القيم
            ResetFields();
        }
        private void ResetFields()
        {
            Date = DateTime.Now;
            ReleaseNumber = string.Empty;
            Value = 0;
            Vat = 0;
            ProfitTax = 0;
            ImportTax = 0;
            Fees = 0;
            Total = 0;
            EPaymentNumber = string.Empty;
            Description = string.Empty;
        }

        // ✅ دالة الحذف
        public void DeleteExpense()
        {
            if (SelectedExpense == null)
            {
                MessageBox.Show("يرجى تحديد صف للحذف.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("هل أنت متأكد من حذف هذا المصروف؟", "تأكيد الحذف",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.ExternalExpenses.Remove(SelectedExpense);
                _context.SaveChanges();
                Expenses.Remove(SelectedExpense);

                MessageBox.Show("تم حذف المصروف بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }


}
