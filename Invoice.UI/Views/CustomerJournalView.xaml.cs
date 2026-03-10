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
using Invoice.UI.ViewModels;

namespace Invoice.UI.Views
{
    public partial class CustomerJournalView : Window
    {
        public CustomerJournalView(CustomerJournalViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

