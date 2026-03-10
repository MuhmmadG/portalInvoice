using Invoice.Core.Model;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.UI.ViewModels
{
    public class TaxReportViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }

        // ✅ المشتريات الداخلية
       // public decimal PurchasesTotal { get; private set; }
        public decimal PurchasesT1 { get; private set; }

        // ✅ المبيعات
        public decimal SalesTotal { get; private set; }
        public decimal SalesT1 { get; private set; }

        // ✅ المصروفات الخارجية
        public decimal ExternalTotal { get; private set; }
        public decimal ExternalVat { get; private set; }

        // ✅ المجموع النهائي (المشتريات الداخلية + الخارجية)
        public decimal CombinedTotal => PurchasesTotal + ExternalTotal;
        public decimal CombinedVat => PurchasesT1 + ExternalVat;
        public decimal PurchasesTotal => PurchasesT1 * 100 / 14;

        // ✅ الفروق (المبيعات - (المشتريات + الخارجي))
        public decimal DifferenceSalesPurchases => SalesTotal - CombinedTotal;
        public decimal DifferenceT1 => SalesT1 - CombinedVat;

        public ObservableCollection<int> Months { get; } =
            new ObservableCollection<int>(Enumerable.Range(1, 12));

        public ObservableCollection<int> Years { get; } =
            new ObservableCollection<int>(Enumerable.Range(2020, 10)); // عدّل حسب احتياجك

        public TaxReportViewModel(AppDbContext context)
        {
            _context = context;

            // قيمة افتراضية: الشهر الحالي والسنة الحالية
            SelectedMonth = DateTime.Now.Month;
            SelectedYear = DateTime.Now.Year;
        }

        public void LoadReport()
        {
            var startDate = new DateTime(SelectedYear, SelectedMonth, 1);
            var endDate = startDate.AddMonths(1);

            // ✅ المشتريات الداخلية (غير دائنة)
            var validPurchases = _context.Documents
                .Where(d => d.Id == d.IssuerId
                         && EF.Functions.Collate(d.TypeVersionName, "SQL_Latin1_General_CP1_CI_AS") != "c"
                         && d.DateTimeReceived >= startDate
                         && d.DateTimeReceived < endDate)
                .Include(d => d.TaxTotals)
                .ToList();

            // 🟥 الفواتير الدائنة للمشتريات
            var creditPurchases = _context.Documents
                .Where(d => d.Id == d.IssuerId
                         && EF.Functions.Collate(d.TypeVersionName, "SQL_Latin1_General_CP1_CI_AS") == "c"
                         && d.DateTimeReceived >= startDate
                         && d.DateTimeReceived < endDate)
                .Include(d => d.TaxTotals)
                .ToList();

            //PurchasesTotal = PurchasesT1*100/14;
            /*validPurchases.Sum(i => i.NetAmount) - creditPurchases.Sum(i => i.NetAmount);*/
            PurchasesT1 = validPurchases.Sum(i => i.TaxTotals.Where(t => t.TaxType == "T1").Sum(t => t.Amount))
                          - creditPurchases.Sum(i => i.TaxTotals.Where(t => t.TaxType == "T1").Sum(t => t.Amount));

            // ✅ المصروفات الخارجية
            var externalExpenses = _context.ExternalExpenses
                .Where(e => e.Date >= startDate && e.Date < endDate)
                .ToList();

            ExternalTotal = (decimal)externalExpenses.Sum(e => e.Total);
            ExternalVat = (decimal)externalExpenses.Sum(e => e.Vat);

            // ✅ المبيعات (غير مرفوضة ولا دائنة)
            var validSales = _context.Documents
                .Where(d => d.Id == d.ReceiverId
                         && EF.Functions.Collate(d.TypeVersionName, "SQL_Latin1_General_CP1_CI_AS") != "c"
                         && EF.Functions.Collate(d.Status, "SQL_Latin1_General_CP1_CI_AS") != "Rejected"
                         && d.DateTimeReceived >= startDate
                         && d.DateTimeReceived < endDate)
                .Include(d => d.TaxTotals)
                .ToList();

            // 🟥 الفواتير الدائنة للمبيعات
            var creditSales = _context.Documents
                .Where(d => d.Id == d.ReceiverId
                         && EF.Functions.Collate(d.TypeVersionName, "SQL_Latin1_General_CP1_CI_AS") == "c"
                         && d.DateTimeReceived >= startDate
                         && d.DateTimeReceived < endDate)
                .Include(d => d.TaxTotals)
                .ToList();

            SalesTotal = validSales.Sum(i => i.NetAmount) - creditSales.Sum(i => i.NetAmount);
            SalesT1 = validSales.Sum(i => i.TaxTotals.Where(t => t.TaxType == "T1").Sum(t => t.Amount))
                      - creditSales.Sum(i => i.TaxTotals.Where(t => t.TaxType == "T1").Sum(t => t.Amount));

            // ✅ تحديث واجهة المستخدم
            OnPropertyChanged(nameof(PurchasesTotal));
            OnPropertyChanged(nameof(PurchasesT1));
            OnPropertyChanged(nameof(ExternalTotal));
            OnPropertyChanged(nameof(ExternalVat));
            OnPropertyChanged(nameof(SalesTotal));
            OnPropertyChanged(nameof(SalesT1));
            OnPropertyChanged(nameof(CombinedTotal));
            OnPropertyChanged(nameof(CombinedVat));
            OnPropertyChanged(nameof(DifferenceSalesPurchases));
            OnPropertyChanged(nameof(DifferenceT1));
        }
    }





}
