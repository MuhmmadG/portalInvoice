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
    /// Interaction logic for ExternalPurchaseReportView.xaml
    /// </summary>
    public partial class ExternalPurchaseReportView : Window
    {
        public ExternalPurchaseReportView(ExternalPurchaseReportViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += async (s, e) => await vm.LoadDescriptionsAsync();
        }

    }
}
