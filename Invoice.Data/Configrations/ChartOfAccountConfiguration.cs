using System;
using System.Collections.Generic;
using System.Text;
using Invoice.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Invoice.Data.Configrations
{
    

    public class ChartOfAccountConfiguration : IEntityTypeConfiguration<ChartOfAccount>
    {
        public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
        {
            builder.ToTable("ChartOfAccounts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.CodeAccount)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.AccountName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.FinancialStatement)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Level)
                .IsRequired();

            builder.Property(a => a.AccountType)
                .IsRequired();

            // العلاقة الشجرية (Self Reference)
            builder.HasOne(a => a.ParentAccount)
                .WithMany(a => a.Children)
                .HasForeignKey(a => a.ParentAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع FinancialTransaction
            builder.HasMany(a => a.FinancialTransactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع TaxTotal
            builder.HasMany(a => a.TaxTotals)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // منع تكرار الكود
            builder.HasIndex(a => a.CodeAccount)
                .IsUnique();
        }
    }
}
