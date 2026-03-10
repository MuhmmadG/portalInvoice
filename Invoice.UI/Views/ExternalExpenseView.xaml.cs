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
    /// Interaction logic for ExternalExpenseView.xaml
    /// </summary>
    public partial class ExternalExpenseView : Window
    {
        public ExternalExpenseView()
        {
            InitializeComponent();
            DataContext = AppHost.GetService<ExternalExpenseViewModel>();
        }
        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ExternalExpenseViewModel vm)
            {
                vm.AddExpense();
                MessageBox.Show("✅ تم إضافة المصروف الخارجي بنجاح", "تم الحفظ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void DeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ExternalExpenseViewModel vm)
            {
                vm.DeleteExpense();
            }
        }

    }
}
