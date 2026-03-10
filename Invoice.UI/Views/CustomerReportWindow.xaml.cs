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
    /// Interaction logic for CustomerReportWindow.xaml
    /// </summary>
    public partial class CustomerReportWindow : Window
    {
        private readonly CustomerReportViewModel _viewModel;
        public CustomerReportWindow()
        {
            InitializeComponent();
            _viewModel = new CustomerReportViewModel();
            DataContext = _viewModel;
            
            Loaded += CustomerReportWindow_Loaded;
        }

        private async void CustomerReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadReportsAsync();
        }

    }
}
