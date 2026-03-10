using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Invoice.Data.Data;
using Invoice.Data.Factories;
using Invoice.Data.RopositoriesStrategy;
using Invoice.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
public class InvoiceViewModel : INotifyPropertyChanged
{

    #region Fields

    private readonly AppDbContext _dbContext;
    private readonly IInvoiceSaveStrategyFactory _factory;
    private readonly IInvoiceReaderFile reader = InvoiceReaderFactory.Create("xml");
    private IInvoiceStrategy strategy;
    private readonly DocumentMapper _mapper;
    private ObservableCollection<DocumentModelDto> _invoices;
    private string _folderPath;
    private string _excelPath;
    private DocumentModelDto _selectedInvoice;

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

    public DocumentModelDto SelectedInvoice
    {
        get => _selectedInvoice;
        set
        {
            _selectedInvoice = value;
            OnPropertyChanged(nameof(SelectedInvoice));
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

    #endregion

    #region Commands

    public ICommand SaveInvoiceCommand { get; }
    public ICommand SaveInvoiceCommandPurshases { get; }
    public ICommand SelectFolderCommand { get; private set; }
    public ICommand ShowInvoicesCommand { get; }
    public ICommand SaveDataBaseCommand { get; }

    #endregion

    #region Constructor

    public InvoiceViewModel(IInvoiceSaveStrategyFactory factory , AppDbContext dbContext, DocumentMapper mapper)
    {
        _factory = factory;

        SaveInvoiceCommandPurshases = new RelayCommand(
            async _ => await SaveInvoiceAsyncPurshases(),
            _ => !string.IsNullOrWhiteSpace(FolderPath)
        );

        SaveInvoiceCommand = new RelayCommand(
            async _ => await SaveInvoiceAsync(),
            _ => !string.IsNullOrWhiteSpace(FolderPath)
        );

        SelectFolderCommand = new RelayCommand(_ => SelectFolder());

        ShowInvoicesCommand = new RelayCommand(
            _ => LoadInvoices(),
            _ => !string.IsNullOrWhiteSpace(FolderPath)
        );
        SaveDataBaseCommand = new RelayCommand(
            async _ => await SaveToDatabaseAsync(),
            _ => Invoices != null && Invoices.Any()
        );
        _dbContext = dbContext;
        _mapper = mapper;
    }

    #endregion

    #region Methods - Save Invoices

    private async Task SaveInvoiceAsync()
    {
        SelectExcelFile();
        var documents = reader.InvoiceReaderFiles(FolderPath);

        strategy = InvoiceFactory.CreateInvoiceStrategy(
            ExcelPath,
            InvoiceType.Sales,
            _factory
        );

        await strategy.SaveAsync(documents);
    }

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

    #endregion

    #region Methods - Load & Select

    public void LoadInvoices()
    {
        var docs = reader.InvoiceReaderFiles(FolderPath);
        Invoices = new ObservableCollection<DocumentModelDto>(docs);
        // تحديث الملخص
        InvoiceCount = Invoices.Count;
        SalesTotal = Invoices.Sum(i => i.TotalAmount);
        VatTotal = Invoices.Sum(i => i.InvoiceLines.Sum(l =>
                      l.TaxableItems.Where(t => t.TaxType == "T1").Sum(t => t.Amount)));
        Tax1Total = Invoices.Sum(i => i.InvoiceLines.Sum(l =>
                      l.TaxableItems.Where(t => t.TaxType == "T4").Sum(t => t.Amount)));
        GrandTotal = Invoices.Sum(i => i.TotalSales);
    }
    private async Task SaveToDatabaseAsync()
    {
        await _mapper.MapAndSaveDocumentSale(Invoices.ToList());
        MessageBox.Show("✅ تم حفظ جميع الفواتير بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
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


