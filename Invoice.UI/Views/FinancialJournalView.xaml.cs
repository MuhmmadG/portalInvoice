using Invoice.Data.Data;
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
    /// Interaction logic for FinancialJournalView.xaml
    /// </summary>
    public partial class FinancialJournalView : Window
    {
        public FinancialJournalView(FinancialJournalViewModel viewModel )
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;   // إلغاء الإغلاق
            this.Hide();       // فقط إخفاء النافذة
        }
    }
}
