using Invoice.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Invoice.UI.Views
{
    /// <summary>
    /// Interaction logic for ProductReportWindow.xaml
    /// </summary>
    public partial class ProductReportWindow : Window
    {
        private readonly ProductReportViewModel _viewModel;

        public ProductReportWindow()
        {
            InitializeComponent();
            _viewModel = new ProductReportViewModel();
            DataContext = _viewModel;
            Loaded += ProductReportWindow_Loaded;
        }

        private async void ProductReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadReportsAsync();
        }
    }

}
