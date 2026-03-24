using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Invoice.Data.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Data.Services
{
    public class FinancialTransactionService : IFinancialTransactionService
    {
        private readonly AppDbContext _context;

        public FinancialTransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTransactionAsync(FinancialTransaction transaction)
        {
            // 🔥 التحقق من الحساب
            if (transaction.AccountId != null)
            {
                var account = await _context.ChartOfAccounts
                    .FirstOrDefaultAsync(a => a.Id == transaction.AccountId);

                if (account == null)
                    throw new Exception("الحساب غير موجود");

                if (!account.IsPosting)
                    throw new Exception("لا يمكن القيد على حساب رئيسي");
            }

            _context.FinancialTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
