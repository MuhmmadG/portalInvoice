using DocumentFormat.OpenXml.Spreadsheet;
using Invoice.Core.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MarkerType = OxyPlot.MarkerType;
namespace Invoice.UI.ViewModels
{
    public class CustomerDashboardViewModel : INotifyPropertyChanged
    {
        private readonly List<CustomerReportDto> _allReports;

        // ==================== FILTERS ====================

        public ObservableCollection<string> AvailableItems { get; set; }
        public ObservableCollection<int> AvailableYears { get; set; }
        public ObservableCollection<int> AvailableMonths { get; set; }

        private string _selectedItem;
        public string SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnFilterChanged(); }
        }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set { _selectedYear = value; OnFilterChanged(); }
        }

        private int _selectedMonth;
        public int SelectedMonth
        {
            get => _selectedMonth;
            set { _selectedMonth = value; OnFilterChanged(); }
        }

        // ==================== KPI ====================

        private string _totalSalesKPI;
        public string TotalSalesKPI
        {
            get => _totalSalesKPI;
            set { _totalSalesKPI = value; OnPropertyChanged(); }
        }

        private string _bestItemKPI;
        public string BestItemKPI
        {
            get => _bestItemKPI;
            set { _bestItemKPI = value; OnPropertyChanged(); }
        }

        private string _totalQuantityKPI;
        public string TotalQuantityKPI
        {
            get => _totalQuantityKPI;
            set { _totalQuantityKPI = value; OnPropertyChanged(); }
        }

        // ==================== CHART MODELS ====================

        private PlotModel _priceTrendModel;
        public PlotModel PriceTrendModel
        {
            get => _priceTrendModel;
            set { _priceTrendModel = value; OnPropertyChanged(); }
        }

        private PlotModel _quantityTrendModel;
        public PlotModel QuantityTrendModel
        {
            get => _quantityTrendModel;
            set { _quantityTrendModel = value; OnPropertyChanged(); }
        }

        private PlotModel _itemsPieModel;
        public PlotModel ItemsPieModel
        {
            get => _itemsPieModel;
            set { _itemsPieModel = value; OnPropertyChanged(); }
        }

        // ==================== CONSTRUCTOR ====================

        public CustomerDashboardViewModel(List<CustomerReportDto> reports)
        {
            _allReports = reports;

            AvailableItems = new ObservableCollection<string>(_allReports.Select(x => x.InternalCode).Distinct());
            AvailableYears = new ObservableCollection<int>(_allReports.Select(x => x.Year).Distinct());
            AvailableMonths = new ObservableCollection<int>(_allReports.Select(x => x.Month).Distinct());

            SelectedItem = AvailableItems.FirstOrDefault();
            SelectedYear = AvailableYears.FirstOrDefault();
            SelectedMonth = -1;

            BuildDashboard();
        }

        // ==================== FILTER ====================

        private void OnFilterChanged()
        {
            BuildDashboard();
        }

        private List<CustomerReportDto> FilteredReports()
        {
            var data = _allReports.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SelectedItem))
                data = data.Where(x => x.InternalCode == SelectedItem);

            if (SelectedYear > 0)
                data = data.Where(x => x.Year == SelectedYear);

            if (SelectedMonth > 0)
                data = data.Where(x => x.Month == SelectedMonth);

            return data.ToList();
        }

        // ==================== DASHBOARD BUILDER ====================

        private void BuildDashboard()
        {
            var data = FilteredReports();

            BuildKPIs(data);

            PriceTrendModel = BuildPriceTrend(data);
            QuantityTrendModel = BuildQuantityTrend(data);
            ItemsPieModel = BuildPieChart(data);
        }

        // ==================== KPI ====================

        private void BuildKPIs(List<CustomerReportDto> data)
        {
            TotalSalesKPI = data.Sum(x => x.TotalSales).ToString("N2");
            TotalQuantityKPI = data.Sum(x => x.TotalQuantity).ToString("N0");

            BestItemKPI = data
                .GroupBy(x => x.InternalCode)
                .OrderByDescending(g => g.Sum(x => x.TotalQuantity))
                .Select(g => g.Key)
                .FirstOrDefault() ?? "-";
        }

        // ==================== PRICE TREND ====================

        private PlotModel BuildPriceTrend(List<CustomerReportDto> data)
        {
            var model = new PlotModel
            {
                Title = "Price Trend",
                Background = OxyColors.White
            };

            var compData = data.OrderBy(x => x.Month).ToList();

            var catAxis = new CategoryAxis { Position = AxisPosition.Bottom };

            foreach (var item in compData)
                catAxis.Labels.Add($"M{item.Month}");

            model.Axes.Add(catAxis);
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            var series = new LineSeries
            {
                MarkerType = MarkerType.Circle
            };

            for (int i = 0; i < compData.Count; i++)
                series.Points.Add(new DataPoint(i, (double)compData[i].Price));

            model.Series.Add(series);

            return model;
        }

        // ==================== QUANTITY TREND ====================

        private PlotModel BuildQuantityTrend(List<CustomerReportDto> data)
        {
            var top = data
                .GroupBy(x => x.InternalCode)
                .Select(g => new { Item = g.Key, Qty = g.Sum(x => x.TotalQuantity) })
                .OrderByDescending(x => x.Qty)
                .Take(10)
                .ToList();

            var model = new PlotModel
            {
                Background = OxyColors.White
            };

            var catAxis = new CategoryAxis
            {
                Position = AxisPosition.Left
            };

            foreach (var it in top)
                catAxis.Labels.Add(it.Item);

            model.Axes.Add(catAxis);

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0
            });

            var series = new BarSeries
            {
                FillColor = OxyColors.SkyBlue
            };

            foreach (var item in top)
                series.Items.Add(new BarItem { Value = (double)item.Qty });

            model.Series.Add(series);

            return model;
        }

        // ==================== PIE CHART ====================

        private PlotModel BuildPieChart(List<CustomerReportDto> data)
        {
            var allData = data
                .GroupBy(x => x.InternalCode)
                .Select(g => new { Item = g.Key, Sales = g.Sum(x => x.TotalSales) })
                .OrderByDescending(x => x.Sales)
                .ToList();

            var totalSales = (double)allData.Sum(x => x.Sales);

            var model = new PlotModel
            {
                Background = OxyColors.White
            };

            var pie = new PieSeries
            {
                Stroke = OxyColors.White
            };

            foreach (var item in allData.Take(10))
            {
                double percentage = totalSales == 0
                    ? 0
                    : ((double)item.Sales / totalSales) * 100;

                pie.Slices.Add(new PieSlice(item.Item, percentage));
            }

            model.Series.Add(pie);

            return model;
        }

        // ==================== NOTIFY ====================

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

