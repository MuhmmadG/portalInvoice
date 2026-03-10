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

        // ==================== FILTER PROPERTIES ====================

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

        // ==================== KPI PROPERTIES ====================

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


        // ==================== CHART PROPERTIES ====================

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

        // Add this property to the class to fix CS0103
        private PlotModel _topItemsModel;
        public PlotModel TopItemsModel
        {
            get => _topItemsModel;
            set { _topItemsModel = value; OnPropertyChanged(); }
        }

        // ==================== CONSTRUCTOR ====================

        public CustomerDashboardViewModel(List<CustomerReportDto> reports)
        {
            _allReports = reports;

            AvailableItems = new ObservableCollection<string>(_allReports.Select(x => x.InternalCode).Distinct());
            AvailableYears = new ObservableCollection<int>(_allReports.Select(x => x.Year).Distinct());
            AvailableMonths = new ObservableCollection<int>(_allReports.Select(x => x.Month).Distinct());

            // Default filters
            SelectedItem = AvailableItems.FirstOrDefault();
            SelectedYear = AvailableYears.FirstOrDefault();
            SelectedMonth = -1; // (All months)

            BuildDashboard();
        }

        // ==================== FILTER LOGIC ====================

        private void OnFilterChanged()
        {
            BuildDashboard();
        }

        private IEnumerable<CustomerReportDto> FilteredReports()
        {
            var data = _allReports.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SelectedItem))
                data = data.Where(x => x.InternalCode == SelectedItem);

            if (SelectedYear > 0)
                data = data.Where(x => x.Year == SelectedYear);

            if (SelectedMonth > 0)
                data = data.Where(x => x.Month == SelectedMonth);

            return data;
        }

        // ==================== BUILD DASHBOARD ====================

        private void BuildDashboard()
        {
            var data = FilteredReports().ToList();

            BuildKPIs(data);
            BuildPriceTrend(data);
            BuildQuantityTrend();
            BuildPieChart();
        }

        // ==================== KPI BUILDER ====================

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

        // ==================== PRICE TREND (Line Chart) ====================

        private void BuildPriceTrend(List<CustomerReportDto> data)
        {
            var model = new PlotModel
            {
                Title = "Price Trend",
                Background = OxyColors.White,
                TextColor = OxyColors.Black
            };

            var compData = data.OrderBy(x => x.Month).ToList();

            var catAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                TextColor = OxyColors.Black
            };

            // فقط Labels.Add — بدون ItemsSource
            foreach (var item in compData)
                catAxis.Labels.Add($"M{item.Month}");

            model.Axes.Add(catAxis);

            var valAxis = new LinearAxis { Position = AxisPosition.Left, TextColor = OxyColors.Black };
            model.Axes.Add(valAxis);

            var series = new LineSeries
            {
                Color = OxyColors.SteelBlue,
                StrokeThickness = 3,
                MarkerType = MarkerType.Circle,
                MarkerSize = 5
            };

            for (int i = 0; i < compData.Count; i++)
                series.Points.Add(new DataPoint(i, (double)compData[i].Price));

            model.Series.Add(series);

            PriceTrendModel = model;
        }

        // ==================== TOP ITEMS (Bar Chart) ====================

        private void BuildQuantityTrend()
        {
            try
            {
                // استخدام جميع البيانات بدون فلترة
                var top = _allReports // <-- كل البيانات
                    .GroupBy(x => x.InternalCode)
                    .Select(g => new { Item = g.Key, Qty = g.Sum(x => x.TotalQuantity) })
                    .OrderByDescending(x => x.Qty) // أعلى الأصناف أولاً
                    .Take(50) // أخذ أعلى 5 أصناف 
                    .ToList();

                // تحقق من وجود بيانات
                if (!top.Any())
                {
                    System.Diagnostics.Debug.WriteLine("No data available for Top Items chart.");
                    return;
                }

                // إنشاء نموذج الرسم البياني
                var model = new PlotModel
                {
                   // Title = "📦 أعلى 5 أصناف من حيث الكمية",
                    Background = OxyColors.White,
                    TextColor = OxyColors.Black
                };

                // المحور التصنيفي (Y-axis)
                var categoryAxis = new CategoryAxis
                {
                    Position = AxisPosition.Left,
                    GapWidth = 0.3,
                    IsTickCentered = true
                };
                foreach (var it in top)
                    categoryAxis.Labels.Add(it.Item);
                model.Axes.Add(categoryAxis);

                // المحور العددي (X-axis)
                var valueAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    MinimumPadding = 0,
                    AbsoluteMinimum = 0,
                    Title = "الكمية"
                };
                model.Axes.Add(valueAxis);

                // إضافة الـ BarSeries
                var series = new BarSeries
                {
                    ItemsSource = top.Select(t => new BarItem { Value = (double)t.Qty }).ToList(),
                    FillColor = OxyColors.SkyBlue
                };
                model.Series.Add(series);

                // تعيين النموذج إلى الخاصية المرتبطة بالـ PlotView
                TopItemsModel = model!;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Quantity Trend Error:\n" + ex.ToString());
            }
        }


        // ==================== PIE CHART ====================

        private void BuildPieChart()
        {
            try
            {
                // استخدام كل البيانات بدون فلترة
                var allData = _allReports
                    .GroupBy(x => x.InternalCode)
                    .Select(g => new { Item = g.Key, Sales = g.Sum(x => x.TotalSales) })
                    .OrderByDescending(x => x.Sales)
                    .ToList();

                if (!allData.Any())
                {
                    System.Diagnostics.Debug.WriteLine("No data available for Pie Chart.");
                    return;
                }

                // احسب مجموع المبيعات الكلي
                double totalSales = (double)allData.Sum(x => x.Sales);

                // قسم البيانات: أعلى 5 أصناف + باقي الأصناف تحت "Others"
                var top5 = allData.Take(50).ToList();
                var others = allData.Skip(50).ToList();
                double othersSum = (double)others.Sum(x => x.Sales);

                // إنشاء نموذج الرسم البياني
                var model = new PlotModel
                {
                   // Title = "🍰 توزيع المبيعات حسب الأصناف (Top 5 + Others)",
                    Background = OxyColors.White,
                    TextColor = OxyColors.Black
                };

                // إنشاء PieSeries
                var pie = new PieSeries
                {
                    Stroke = OxyColors.White,
                    InsideLabelColor = OxyColors.Black,
                    AngleSpan = 360,
                    StartAngle = 0,
                   // InsideLabelFormat = "{1:0.0}%" // عرض النسبة المئوية داخل القطعة
                };

                // إضافة أعلى 10 أصناف
                foreach (var item in top5)
                {
                    double percentage = totalSales == 0 ? 0 : ((double)item.Sales / (double)totalSales) * 100;

                    pie.Slices.Add(new PieSlice(item.Item, percentage) { IsExploded = false });
                }

                // إضافة "Others" إذا كان موجود
                if (othersSum > 0)
                {
                    double othersPercentage = (othersSum / totalSales) * 100;
                    pie.Slices.Add(new PieSlice("Others", othersPercentage) { IsExploded = false, Fill = OxyColors.LightGray });
                }

                model.Series.Add(pie);

                // تعيين النموذج للـ PlotView
                ItemsPieModel = model!;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Pie Chart Error:\n" + ex.ToString());
            }
        }






        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

