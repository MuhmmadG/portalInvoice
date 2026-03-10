using GalaSoft.MvvmLight.Command;
using Invoice.Core.Model;
using Invoice.Data;
using Invoice.Data.Data;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

public class ExpenseAnalysisViewModel : BaseViewModel
{
    private DateTime? _dateFrom;
    public DateTime? DateFrom
    {
        get => _dateFrom;
        set
        {
            if (SetProperty(ref _dateFrom, value))
                LoadData();
        }
    }

    private DateTime? _dateTo;
    public DateTime? DateTo
    {
        get => _dateTo;
        set
        {
            if (SetProperty(ref _dateTo, value))
                LoadData();
        }
    }

    private readonly AppDbContext _context;
    public ICommand ExportToExcelCommand { get; }

    public ExpenseAnalysisViewModel(AppDbContext context)
    {
        _context = context;
        LoadCategories();
        LoadData();
        ExportToExcelCommand = new RelayCommand(ExportToExcel);

    }
    private void ExportToExcel()
    {
        if (Results == null || Results.Count == 0)
        {
            MessageBox.Show("لا توجد بيانات للتصدير.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        // مسار سطح المكتب
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, $"تحليل المصروفات-{DateTime.Now:yyyyMMddHHmm}.xlsx");

        OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Muhammed Gamal");

        using (var package = new OfficeOpenXml.ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("Expenses");

            ws.Cells["A1"].Value = "التاريخ";
            ws.Cells["B1"].Value = "المورد";
            ws.Cells["C1"].Value = "رقم داخلي";
            ws.Cells["D1"].Value = "الصافي";
            ws.Cells["E1"].Value = "الضريبة";
            ws.Cells["F1"].Value = "الإجمالي";

            int row = 2;

            foreach (var item in Results)
            {
                ws.Cells[row, 1].Value = item.Date;
                ws.Cells[row, 2].Value = item.SupplierName;
                ws.Cells[row, 3].Value = item.InternalId;
                ws.Cells[row, 4].Value = item.NetAmount;
                ws.Cells[row, 5].Value = item.TaxAmount;
                ws.Cells[row, 6].Value = item.Total;

                row++;
            }

            ws.Cells.AutoFitColumns();

            package.SaveAs(new FileInfo(filePath));
        }

        MessageBox.Show($"✔ تم حفظ الملف على سطح المكتب:\n{filePath}",
                        "تم بنجاح", MessageBoxButton.OK, MessageBoxImage.Information);
    }


    // -----------------------------
    // ComboBox (أنواع المصروفات)
    // -----------------------------
    public ObservableCollection<string> Categories { get; set; } = new();

    private string _selectedCategory;
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
                LoadData(); // تحديث البيانات عند تغيير النوع
        }
    }

    // -----------------------------
    // قائمة النتائج
    // -----------------------------
    public ObservableCollection<ExpenseAnalysisModel> Results { get; set; } = new();

    // -----------------------------
    // تحميل أنواع المصروفات من ExpenseCategories
    // -----------------------------
    private void LoadCategories()
    {
        Categories.Clear();

        var cats = _context.ExpenseCategories
                           .Select(x => x.CategoryType)
                           .Distinct()
                           .ToList();

        foreach (var c in cats)
            Categories.Add(c);
    }

    // -----------------------------
    // تنفيذ الاستعلام
    // -----------------------------
    private void LoadData()
    {
        Results.Clear();

        if (string.IsNullOrWhiteSpace(SelectedCategory))
            return;

        var query =
            from D in _context.Documents
            join P in _context.Parties on D.IssuerId equals P.Id
            join EX in _context.ExpenseCategories on D.ExpenseCategoryId equals EX.Id
            join T in _context.TaxTotals on D.Id equals T.DocumentModelId
            where EX.CategoryType == SelectedCategory
            select new
            {
                D.DateTimeReceived,
                P.Name,
                D.InternalId,
                D.NetAmount,
                T.Amount,
                D.Total
            };

        // فلترة التاريخ
        if (DateFrom.HasValue)
            query = query.Where(x => x.DateTimeReceived >= DateFrom.Value);

        if (DateTo.HasValue)
            query = query.Where(x => x.DateTimeReceived <= DateTo.Value);

        var final = query.ToList();

        foreach (var item in final)
        {
            Results.Add(new ExpenseAnalysisModel
            {
                Date = item.DateTimeReceived,
                SupplierName = item.Name,
                InternalId = item.InternalId,
                NetAmount = item.NetAmount,
                TaxAmount = item.Amount,
                Total = item.Total
            });
        }
    }
    // -----------------------------
    //load data
    //-----------------------------


}
