using Invoice.Core.Model;
using Invoice.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
namespace Invoice.Data.Data
{
    public class AppDbContext : DbContext
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        // DbSets
        // Constructor إضافي للـ design-time
        public AppDbContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // إذا لم يكن هناك إعدادات مسبقة، استخدم الاتصال الافتراضي
            if (!options.IsConfigured)
            {
                options.UseSqlServer(AppConfiguration.GetConnectionString());
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configrations.DocumentModelConfigration());
            modelBuilder.ApplyConfiguration(new Configrations.PartyConfigration());
            modelBuilder.ApplyConfiguration(new Configrations.AddressConfigration());
            modelBuilder.ApplyConfiguration(new Configrations.InvoiceLineConfigration());
            modelBuilder.ApplyConfiguration(new Configrations.TaxTotalConfigration());
            modelBuilder.ApplyConfiguration(new Configrations.ExpenseCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new Configrations.TaxableItemConfigration());
            modelBuilder.ApplyConfiguration(new Configrations.ItemCodeConfigConfiguration());
            modelBuilder.ApplyConfiguration(new Configrations.ImportedItemConfiguration());
            modelBuilder.ApplyConfiguration(new Configrations.ExternalExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new Configrations.OtherExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new Configrations.FinancialTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new Configrations.ChartOfAccountConfiguration());
            modelBuilder.ApplyConfiguration(new Configrations.JournalEntryConfiguration());
            modelBuilder.ApplyConfiguration(new Configrations.JournalEntryLineConfiguration());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerBalanceView>(entity =>
            {
                entity.HasNoKey(); // لأنه View
                entity.ToView("View_CustomerBalances");
            });
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SupplierBalanceView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("View_SupplierBalances");
            });
        }
        public DbSet<Invoice.Core.Model.DocumentModel> Documents { get; set; }
        public DbSet<Invoice.Core.Model.Party> Parties { get; set; }
        public DbSet<Invoice.Core.Model.Address> Addresses { get; set; }
        public DbSet<Invoice.Core.Model.InvoiceLine> InvoiceLines { get; set; }
        public DbSet<Invoice.Core.Model.TaxTotal> TaxTotals { get; set; }
        public DbSet<Invoice.Core.Model.ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Invoice.Core.Model.TaxableItem> TaxableItem { get; set; }
        public DbSet<Invoice.Core.Model.ItemCodeConfig> ItemCodeConfig { get; set; }
        public DbSet<Invoice.Core.Model.ImportedItem> ImportedItems { get; set; }
        public DbSet<Invoice.Core.Model.ExternalExpense> ExternalExpenses { get; set; }
        public DbSet<ItemCodeConfig> ItemMappings => Set<ItemCodeConfig>();
        public DbSet<OtherExpense> OtherExpenses { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<CustomerBalanceView> CustomerBalances { get; set; }
        public DbSet<SupplierBalanceView> SupplierBalances { get; set; }
        public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
        }
        
}
