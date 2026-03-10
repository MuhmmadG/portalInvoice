using GalaSoft.MvvmLight;
using Invoice.Core.Model;
using Invoice.Data.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Invoice.UI.ViewModels
{
    public class CustomerBalancesViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;

        public ObservableCollection<CustomerBalanceView> Balances { get; set; }

        public CustomerBalancesViewModel(AppDbContext context)
        {
            _context = context;

            LoadBalances();
        }

        private void LoadBalances()
        {
            var data = _context.CustomerBalances.ToList();

            Balances = new ObservableCollection<CustomerBalanceView>(data);
        }
    }
}
