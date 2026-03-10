using Invoice.Core.Model;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.UI.ViewModels
{
    public class CustomerReportViewModel : BaseViewModel
    {
       
        private ObservableCollection<CustomerReportDto> _reports;
        public List<CustomerReportDto> _allReports; // نخزن البيانات كلها هنا
        public List<CustomerReportDto> AllReports => _allReports ?? new List<CustomerReportDto>();

        public ObservableCollection<CustomerReportDto> Reports
        {
            get => _reports;
            set => SetProperty(ref _reports, value);
        }

        public List<int> Years { get; set; } =  Enumerable.Range(2024, 2025).ToList();
        public List<int> Months { get; set; } = Enumerable.Range(1, 12).ToList();

        private int? _selectedYear;
        public int? SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (SetProperty(ref _selectedYear, value))
                    ApplyFilter();
            }
        }

        private int? _selectedMonth;
        public int? SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                    ApplyFilter();
            }
        }

      
        public async Task LoadReportsAsync()
        {
            using (var context = new AppDbContext())
            {
                var data = await context.Parties
                    .Join(context.Documents
                              .Where(d => d.TypeVersionName != "C" && d.TypeVersionName != "c"), // ✅ استبعاد الفواتير المرتجعة
                          p => p.Id,
                          d => d.ReceiverId,
                          (p, d) => new { p, d })
                    .Join(context.InvoiceLines,
                          pd => pd.d.Id,
                          i => i.DocumentModelId,
                          (pd, i) => new { pd.p, pd.d, i })
                    .Join(context.ItemMappings,
                          pdi => pdi.i.ItemMappingId,
                          it => it.Id,
                          (pdi, it) => new { pdi.p, pdi.d, pdi.i, it })
                    .GroupBy(x => new
                    {
                        x.p.Name,
                        Year = x.d.DateTimeReceived.Year,
                        Month = x.d.DateTimeReceived.Month,
                        x.it.InternalCode,
                        x.it.Kind,
                        x.i.unitValue.AmountEGP,
                    })
                    .Select(g => new CustomerReportDto
                    {
                        CustomerName = g.Key.Name,
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        InternalCode = g.Key.InternalCode,
                        Kind = g.Key.Kind,
                        Price = g.Key.AmountEGP,
                        TotalQuantity = g.Sum(x => x.i.quantity),
                        TotalSales = g.Sum(x => x.i.netTotal),
                        GrandTotal = g.Sum(x => x.i.total),
                        TotalTax = g.Sum(x => x.i.TaxableItems
                            .Where(t => t.TaxType == "T1")
                            .Sum(t => t.Amount))
                    })
                    .OrderBy(r => r.CustomerName)
                    .ThenBy(r => r.Year)
                    .ThenBy(r => r.Month)
                    .ThenBy(r => r.InternalCode)
                    .ThenBy(r => r.Price)
                    .ToListAsync();

                _allReports = data;
                Reports = new ObservableCollection<CustomerReportDto>(_allReports);
                Years = _allReports.Select(r => r.Year).Distinct().OrderBy(y => y).ToList();
            }
        }

        private void ApplyFilter()
        {
            var filtered = _allReports.AsEnumerable();

            if (SelectedYear.HasValue)
                filtered = filtered.Where(r => r.Year == SelectedYear.Value);

            if (SelectedMonth.HasValue)
                filtered = filtered.Where(r => r.Month == SelectedMonth.Value);

            Reports = new ObservableCollection<CustomerReportDto>(filtered);
        }
      
    }

}
