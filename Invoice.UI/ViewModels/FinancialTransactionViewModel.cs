using Invoice.Core.Helpers;
using Invoice.Core.Model;
using Invoice.Data;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
namespace Invoice.UI.ViewModels
{
    public class FinancialTransactionViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        // 🧾 القوائم الأساسية
        public ObservableCollection<FinancialTransaction> Transactions { get; set; } = new();
        public ObservableCollection<Party> Parties { get; set; } = new();
        public ObservableCollection<DocumentModel> Documents { get; set; } = new();

        // 🧮 القوائم الجديدة للفلاتر
        public ObservableCollection<ExpenseCategory> ExpenseCategories { get; set; } = new();
        public ObservableCollection<Party> FilteredParties { get; set; } = new();
        public ObservableCollection<DocumentModel> FilteredDocuments { get; set; } = new();

        public FinancialTransaction SelectedTransaction { get; set; }

        // ==================== الخصائص ====================
        public ObservableCollection<OtherExpense> OtherExpenses { get; set; } = new();

        private OtherExpense _selectedOtherExpense;
        public OtherExpense SelectedOtherExpense
        {
            get => _selectedOtherExpense;
            set
            {
                _selectedOtherExpense = value;
                OnPropertyChanged();
            }
        }
        private void LoadOtherExpenses()
        {
            try
            {
                var expenses = _context.OtherExpenses
                    .Include(e => e.ExpenseCategory)
                    .AsNoTracking()
                    .OrderByDescending(e => e.Date)
                    .ToList();

                OtherExpenses = new ObservableCollection<OtherExpense>(expenses);
                OnPropertyChanged(nameof(OtherExpenses));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل المصروفات الأخرى: {ex.Message}",
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private TransactionType _selectedTransactionType;
        public TransactionType SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                if (_selectedTransactionType != value)
                {
                    _selectedTransactionType = value;
                    OnPropertyChanged();

                    IsPaymentMode = (_selectedTransactionType == TransactionType.Payment);
                    OnPropertyChanged(nameof(IsPaymentMode));

                    // 🧩 إعادة تحميل الأطراف
                    FilterParties();

                    // ✅ تحديث جدول المعاملات مباشرة حسب نوع الحركة
                    LoadTransactions();
                    CalculatePartyBalance();
                }
            }
        }
        public ObservableCollection<OtherExpense> FilteredOtherExpenses { get; set; } = new();

        private void FilterOtherExpenses()
        {
            if (SelectedExpenseCategory == null)
            {
                FilteredOtherExpenses.Clear();
                return;
            }

            var filtered = _context.OtherExpenses
                .Where(o => o.ExpenseCategoryId == SelectedExpenseCategory.Id)
                .OrderByDescending(o => o.Date)
                .ToList();

            FilteredOtherExpenses = new ObservableCollection<OtherExpense>(filtered);
            OnPropertyChanged(nameof(FilteredOtherExpenses));
        }


        private ExpenseCategory _selectedExpenseCategory;
        public ExpenseCategory SelectedExpenseCategory
        {
            get => _selectedExpenseCategory;
            set
            {
                _selectedExpenseCategory = value;
                OnPropertyChanged();
                FilterParties();
                FilterOtherExpenses(); // ✅ التصفية التلقائية
                CalculatePartyBalance();
            }
        }

        private Party _selectedParty;
        public Party SelectedParty
        {
            get => _selectedParty;
            set
            {
                _selectedParty = value;
                OnPropertyChanged();
               // LoadDocumentsByParty();
                CalculatePartyBalance();
            }
        }

        private DocumentModel _selectedDocument;
        public DocumentModel SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                _selectedDocument = value;
                OnPropertyChanged();
            }
        }

        public string ManualPartyName { get; set; }
      
        private string _amountText;
        public string AmountText
        {
            get => _amountText;
            set
            {
                _amountText = value;
                OnPropertyChanged();

                if (decimal.TryParse(value,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var result))
                {
                    Amount = result;
                    ValidationMessage = string.Empty;
                }
                else
                {
                    ValidationMessage = "❌ أدخل رقم صحيح";
                }
            }
        }
        private decimal _partyBalance;
        public decimal PartyBalance
        {
            get => _partyBalance;
            set
            {
                _partyBalance = value;
                OnPropertyChanged();
            }
        }

        public decimal Amount { get; private set; }


        private PaymentMethod _selectedPaymentMethod;
        public PaymentMethod SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                if (_selectedPaymentMethod != value)
                {
                    _selectedPaymentMethod = value;
                    OnPropertyChanged();
                    UpdatePaymentVisibility();
                }
            }
        }

        public string ChequeNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;

        // 🧠 خصائص تحكم بالظهور
        private bool _isChequeFieldsVisible;
        public bool IsChequeFieldsVisible
        {
            get => _isChequeFieldsVisible;
            set { _isChequeFieldsVisible = value; OnPropertyChanged(); }
        }

        private bool _isBankFieldsVisible;
        public bool IsBankFieldsVisible
        {
            get => _isBankFieldsVisible;
            set { _isBankFieldsVisible = value; OnPropertyChanged(); }
        }

        private bool _isPaymentMode;
        public bool IsPaymentMode
        {
            get => _isPaymentMode;
            set { _isPaymentMode = value; OnPropertyChanged(); }
        }
        private string _validationMessage;
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                OnPropertyChanged();
            }
        }


        // ==================== الأوامر ====================
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        // ==================== البناء ====================
        public FinancialTransactionViewModel(AppDbContext context)
        {
            _context = context;

            AddCommand = new RelayCommand(_ => AddTransaction(), _ => CanAddTransaction());

            SaveCommand = new RelayCommand(async _ => await SaveTransactionsAsync());
            DeleteCommand = new RelayCommand(_ => DeleteSelected());

            LoadExpenseCategories();
            LoadTransactions();
            LoadOtherExpenses();

            // ✅ تعيين الافتراضي للسداد
            SelectedTransactionType = TransactionType.Payment;
            IsPaymentMode = true;
        }
        private bool CanAddTransaction()
        {
            // يمكنك إضافة شروط إضافية هنا (مثل التحقق من اختيار المورد أو المستند)
            // لكن في حالتنا نتحقق فقط من المبلغ
            return Amount > 0;
        }

        // ==================== تحميل البيانات ====================

        private void LoadTransactions()
        {
            try
            {
                var data = _context.FinancialTransactions
                    .Include(t => t.Party)
                    .Include(t => t.DocumentModel)
                    .AsNoTracking()
                    .ToList();

                // ✅ فلترة حسب نوع الحركة المحدد (سداد / تحصيل)
                if (SelectedTransactionType == TransactionType.Payment)
                {
                    data = data.Where(t => t.TransactionType == TransactionType.Payment).ToList();
                }
                else if (SelectedTransactionType == TransactionType.Receipt)
                {
                    data = data.Where(t => t.TransactionType == TransactionType.Receipt).ToList();
                }

                Transactions = new ObservableCollection<FinancialTransaction>(data);

                // ✅ إشعار WPF بتغيير المرجع
                OnPropertyChanged(nameof(Transactions));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل المعاملات: {ex.Message}",
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void LoadExpenseCategories()
        {
            ExpenseCategories = new ObservableCollection<ExpenseCategory>(
                _context.ExpenseCategories.AsNoTracking().ToList());
        }
        // ==================== منطق المعاملات ====================
        private void CalculatePartyBalance()
        {
            if (SelectedParty == null)
            {
                PartyBalance = 0;
                return;
            }

            var partyIds = _context.Parties
                .Where(p => p.Name == SelectedParty.Name)
                .Select(p => p.Id)
                .ToList();

            decimal totalInvoices = 0;
            decimal totalTransactions = 0;

            if (SelectedTransactionType == TransactionType.Receipt)
            {
                totalInvoices = _context.Documents
                    .Where(d => d.ReceiverId.HasValue && partyIds.Contains(d.ReceiverId.Value))
                    .Sum(d => (decimal?)(d.TypeVersionName == "C" ? -d.Total : d.Total)) ?? 0;

                totalTransactions = _context.FinancialTransactions
                    .Where(t => t.PartyId.HasValue &&
                                partyIds.Contains(t.PartyId.Value) &&
                                t.TransactionType == TransactionType.Receipt)
                    .Sum(t => (decimal?)t.Amount) ?? 0;
            }
            else if (SelectedTransactionType == TransactionType.Payment)
            {
                totalInvoices = _context.Documents
                    .Where(d => d.IssuerId.HasValue && partyIds.Contains(d.IssuerId.Value))
                    .Sum(d => (decimal?)(d.TypeVersionName == "C" ? -d.Total : d.Total)) ?? 0;

                totalTransactions = _context.FinancialTransactions
                    .Where(t => t.PartyId.HasValue &&
                                partyIds.Contains(t.PartyId.Value) &&
                                t.TransactionType == TransactionType.Payment)
                    .Sum(t => (decimal?)t.Amount) ?? 0;
            }

            PartyBalance = totalInvoices - totalTransactions;

            OnPropertyChanged(nameof(PartyBalance));
        }
        private void AddTransaction()
        {
            if (!ValidationHelper.ValidatePositiveAmount(Amount, out string error))
            {
                ValidationMessage = error;
                return;
            }

            ValidationMessage = string.Empty;

            var t = new FinancialTransaction
            {
                Date = Date,
                TransactionType = SelectedTransactionType,
                PartyId = SelectedParty?.Id,
                ManualPartyName = string.IsNullOrWhiteSpace(ManualPartyName) ? null : ManualPartyName,
                DocumentModelId = SelectedDocument?.Id,
                OtherExpenseId = SelectedOtherExpense?.Id, // ✅ الربط الجديد
                Amount = Amount,
                PaymentMethod = SelectedPaymentMethod,
                ChequeNumber = ChequeNumber,
                DueDate = DueDate,
                BankName = BankName,
                BankAccountNumber = BankAccountNumber,
                Notes = Notes
            };


            Transactions.Add(t);
        }

        private async Task SaveTransactionsAsync()
        {
            var newItems = Transactions.Where(t => t.Id == 0).ToList();

            foreach (var t in newItems)
                _context.FinancialTransactions.Add(t);

            await _context.SaveChangesAsync();

            MessageBox.Show("تم الحفظ بنجاح");
            ClearFields();
            LoadTransactions(); // تحديث الجدول
        }
        private void ClearFields()
        {
            SelectedParty = null;
            SelectedDocument = null;
            SelectedOtherExpense = null;

            AmountText = "";
            Amount = 0;

            ChequeNumber = "";
            BankName = "";
            BankAccountNumber = "";

            Notes = "";

            DueDate = null;

            Date = DateTime.Today;

            OnPropertyChanged(nameof(SelectedParty));
            OnPropertyChanged(nameof(SelectedDocument));
            OnPropertyChanged(nameof(AmountText));
            OnPropertyChanged(nameof(ChequeNumber));
            OnPropertyChanged(nameof(BankName));
            OnPropertyChanged(nameof(BankAccountNumber));
            OnPropertyChanged(nameof(Notes));
        }

        private void DeleteSelected()
        {
            try
            {
                if (SelectedTransaction == null)
                    return;

                // تأكيد الحذف من المستخدم
                var result = MessageBox.Show("هل أنت متأكد من حذف هذه المعاملة؟",
                                             "تأكيد الحذف",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                // 🧠 تأكد من أن EF يتتبع الكائن
                var entity = _context.FinancialTransactions
                    .FirstOrDefault(t => t.Id == SelectedTransaction.Id);

                if (entity != null)
                {
                    _context.FinancialTransactions.Remove(entity);
                    Transactions.Remove(SelectedTransaction);
                    _context.SaveChanges();

                    MessageBox.Show("تم حذف المعاملة بنجاح ✅",
                                    "تم الحذف",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("تعذر العثور على المعاملة في قاعدة البيانات.",
                                    "تنبيه",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء حذف المعاملة:\n{ex.Message}",
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // ==================== التحكم في الظهور ====================

        private void UpdatePaymentVisibility()
        {
            IsChequeFieldsVisible = SelectedPaymentMethod == PaymentMethod.Cheque;
            IsBankFieldsVisible = SelectedPaymentMethod == PaymentMethod.BankTransfer;
        }

        // ==================== منطق التصفية الذكي ====================

        private void FilterParties()
        {
            try
            {
                FilteredParties.Clear();

                if (SelectedExpenseCategory == null)
                    return;

                IQueryable<Party> query;

                // 🧠 لو نوع الحركة سداد => الموردين (Issuer)
                if (SelectedTransactionType == TransactionType.Payment)
                {
                    query = from p in _context.Parties
                            join d in _context.Documents on p.Id equals d.IssuerId
                            where d.ExpenseCategoryId == SelectedExpenseCategory.Id
                            select p;
                }
                // 🧠 لو تحصيل => العملاء (Receiver)
                else
                {

                    query = from p in _context.Parties
                            join d in _context.Documents on p.Id equals d.ReceiverId
                            select p;
                }
                var parties = query
                            .AsNoTracking()
                            .GroupBy(p => p.Name)
                            .Select(g => g.First())
                            .ToList();

                foreach (var p in parties)
                    FilteredParties.Add(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل الأطراف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDocumentsByParty()
        {
            try
            {
                FilteredDocuments.Clear();

                if (SelectedParty == null)
                    return;

                IQueryable<DocumentModel> query;

                // 🧠 لو سداد => الفواتير التي المورد هو مصدرها (Issuer)
                if (SelectedTransactionType == TransactionType.Payment)
                {
                    if (SelectedExpenseCategory == null)
                        return;

                    query = from p in _context.Parties
                            join d in _context.Documents on p.Id equals d.IssuerId
                            where d.ExpenseCategoryId == SelectedExpenseCategory.Id
                                  && p.Name == SelectedParty.Name
                            select d;
                }
                // 🧠 لو تحصيل => الفواتير التي العميل هو مستقبلها (Receiver)
                else
                {
                    query = from p in _context.Parties
                            join d in _context.Documents on p.Id equals d.ReceiverId
                            where p.Name == SelectedParty.Name
                            select d;
                }

                // ⚙️ جلب البيانات أولًا لتفادي الخطأ
                var allDocs = query
                    .AsNoTracking()
                    .OrderByDescending(d => d.DateTimeIssued)
                    .ToList();

                // ✅ إزالة التكرار في الذاكرة بعد الجلب
                var uniqueDocs = allDocs
                    .GroupBy(d => d.InternalId)
                    .Select(g => g.First())
                    .ToList();

                foreach (var doc in uniqueDocs)
                    FilteredDocuments.Add(doc);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل الفواتير الخاصة بالطرف: {ex.Message}",
                    "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }



}
