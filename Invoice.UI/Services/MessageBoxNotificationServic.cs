using Invoice.Core.Interfaces;
using Invoice.Core.Interfaces.Invoice.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace Invoice.UI.Services
{
    public class MessageBoxNotificationService : INotificationService
    {
        public void ShowInfo(string message) =>
            MessageBox.Show(message, "معلومة", MessageBoxButton.OK, MessageBoxImage.Information);

        public void ShowError(string message) =>
            MessageBox.Show(message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
