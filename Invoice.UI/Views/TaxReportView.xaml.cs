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
    /// Interaction logic for TaxReportView.xaml
    /// </summary>
    public partial class TaxReportView : Window
    {
        public TaxReportView()
        {
            InitializeComponent();
        }
        private void LoadReport_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TaxReportViewModel vm)
            {
                vm.LoadReport();
            }
        }

    }
}
