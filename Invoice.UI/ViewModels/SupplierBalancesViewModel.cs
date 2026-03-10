using GalaSoft.MvvmLight;
using Invoice.Core.Model;
using Invoice.Data.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Invoice.UI.ViewModels
{
    public class SupplierBalancesViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;

        public ObservableCollection<SupplierBalanceView> Balances { get; set; }

        public SupplierBalancesViewModel(AppDbContext context)
        {
            _context = context;

            LoadBalances();
        }

        private void LoadBalances()
        {
            var data = _context.SupplierBalances.ToList();

            Balances = new ObservableCollection<SupplierBalanceView>(data);
        }
    }
}
