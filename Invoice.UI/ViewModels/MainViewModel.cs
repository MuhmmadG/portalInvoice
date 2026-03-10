using GalaSoft.MvvmLight.Command;
using Invoice.Core.Interfaces.Invoice.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Invoice.UI.ViewModels
{
    public class MainViewModel
    {
        private readonly INotificationService _notifier;

        public string Title { get; set; } = "مثال WPF مع DI";

        public ICommand ShowMessageCommand { get; }


        public MainViewModel(INotificationService notifier)
        {
            _notifier = notifier;

            ShowMessageCommand = new RelayCommand(_ =>
            {
                _notifier.ShowInfo("تم استدعاء الرسالة من ViewModel!");
            });
        }
    }
}
