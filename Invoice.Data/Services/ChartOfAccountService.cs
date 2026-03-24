using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Invoice.Data.Services
{
    public class ChartOfAccountService : IChartOfAccountService
    {
        private readonly AppDbContext _context;

        public ChartOfAccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChartOfAccount>> GetTreeAsync()
        {
            var allAccounts = await _context.ChartOfAccounts
                .AsNoTracking()
                .ToListAsync();

            return BuildTree(allAccounts);
        }
        private List<ChartOfAccount> BuildTree(List<ChartOfAccount> allAccounts)
        {
            var lookup = allAccounts.ToLookup(a => a.ParentAccountId);

            List<ChartOfAccount> Build(int? parentId)
            {
                return lookup[parentId]
                    .Select(a => new ChartOfAccount
                    {
                        Id = a.Id,
                        AccountName = a.AccountName,
                        CodeAccount = a.CodeAccount,
                        Level = a.Level,
                        AccountType = a.AccountType,
                        FinancialStatement = a.FinancialStatement,
                        ParentAccountId = a.ParentAccountId,
                        IsPosting = a.IsPosting,
                        Children = Build(a.Id)
                    }).ToList();
            }

            return Build(null);
        }
        public async Task<ChartOfAccount> AddAccountAsync(ChartOfAccount account)
        {
            if (account.ParentAccountId != null)
            {
                var parent = await _context.ChartOfAccounts
                    .FirstOrDefaultAsync(a => a.Id == account.ParentAccountId);

                // 1️⃣ تحديد المستوى
                account.Level = parent.Level + 1;

                // 2️⃣ توليد الكود
                var siblings = await _context.ChartOfAccounts
                    .Where(a => a.ParentAccountId == parent.Id)
                    .OrderByDescending(a => a.CodeAccount)
                    .ToListAsync();

                int nextNumber = 1;

                if (siblings.Any())
                {
                    var lastCode = siblings.First().CodeAccount;
                    var lastPart = lastCode.Substring(parent.CodeAccount.Length);
                    nextNumber = int.Parse(lastPart) + 1;
                }

                account.CodeAccount = parent.CodeAccount + nextNumber.ToString("D2");

                // 3️⃣ الأب لم يعد Posting
                parent.IsPosting = false;
            }
            else
            {
                // Root
                account.Level = 1;

                // 🔥 توليد كود Root
                var lastRoot = await _context.ChartOfAccounts
                    .Where(a => a.ParentAccountId == null)
                    .OrderByDescending(a => a.CodeAccount)
                    .FirstOrDefaultAsync();

                int next = 1;

                if (lastRoot != null)
                    next = int.Parse(lastRoot.CodeAccount) + 1;

                account.CodeAccount = next.ToString();
            }

            // 4️⃣ الحساب الجديد Leaf
            account.IsPosting = true;

            _context.ChartOfAccounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task UpdateAccountAsync(ChartOfAccount account)
        {
            var existing = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(a => a.Id == account.Id);

            if (existing == null)
                throw new Exception("الحساب غير موجود");

            existing.AccountName = account.AccountName;
            existing.AccountType = account.AccountType;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(int id)
        {
            var account = await _context.ChartOfAccounts
                .Include(a => a.Children)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account.Children.Any())
                throw new Exception("لا يمكن حذف حساب يحتوي على حسابات فرعية");

            _context.ChartOfAccounts.Remove(account);
            await _context.SaveChangesAsync();
        }
    }
}
