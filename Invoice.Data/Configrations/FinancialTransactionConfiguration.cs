using Invoice.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.Configrations
{
  
        public class FinancialTransactionConfiguration : IEntityTypeConfiguration<FinancialTransaction>
        {
            public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
            {
                // 🧱 اسم الجدول
                builder.ToTable("FinancialTransactions");

                // المفتاح الأساسي
                builder.HasKey(t => t.Id);

                // 📅 التاريخ
                builder.Property(t => t.Date)
                       .IsRequired();

                // نوع الحركة (سداد / تحصيل)
                builder.Property(t => t.TransactionType)
                       .HasConversion<int>()
                       .IsRequired();

                // نوع الدفع (نقدي / شيك / تحويل)
                builder.Property(t => t.PaymentMethod)
                       .HasConversion<int>()
                       .IsRequired();

                // 💵 المبلغ
                builder.Property(t => t.Amount)
                       .HasColumnType("decimal(18,2)")
                       .IsRequired();

                // 🏢 العلاقة مع Party (عميل / مورد)
                builder.HasOne(t => t.Party)
                       .WithMany(p => p.FinancialTransactions)
                       .HasForeignKey(t => t.PartyId)
                       .OnDelete(DeleteBehavior.SetNull);

                // 🔗 العلاقة مع DocumentModel (فاتورة)
                builder.HasOne(t => t.DocumentModel)
                       .WithMany(d => d.FinancialTransactions)
                       .HasForeignKey(t => t.DocumentModelId)
                       .OnDelete(DeleteBehavior.SetNull);

                // 🔤 خصائص نصية اختيارية
                builder.Property(t => t.ManualPartyName)
                       .HasMaxLength(250)
                       .IsUnicode()
                       .IsRequired(false);

                builder.Property(t => t.ChequeNumber)
                       .HasMaxLength(100)
                       .IsRequired(false);

                builder.Property(t => t.BankName)
                       .HasMaxLength(200)
                       .IsUnicode()
                       .IsRequired(false);

                builder.Property(t => t.BankAccountNumber)
                       .HasMaxLength(100)
                       .IsRequired(false);

                builder.Property(t => t.Notes)
                       .HasMaxLength(1000)
                       .IsUnicode()
                       .IsRequired(false);

                // تجاهل الخصائص غير المخزنة
                builder.Ignore(t => t.PaymentMethodDisplay);

                // 🧭 الفهارس لتحسين الأداء
                builder.HasIndex(t => t.Date).HasDatabaseName("IX_FinancialTransactions_Date");
                builder.HasIndex(t => t.PartyId).HasDatabaseName("IX_FinancialTransactions_PartyId");
                builder.HasIndex(t => t.DocumentModelId).HasDatabaseName("IX_FinancialTransactions_DocumentModelId");
            // 1. إعداد علاقة الحساب المحاسبي الأساسي
            builder.HasOne(t => t.Account)             // العملية لها حساب واحد
                   .WithMany()                         // (أو .WithMany(a => a.Transactions) إذا كان لديك قائمة في الـ Account)
                   .HasForeignKey(t => t.AccountId)    // مفتاح الربط
                   .OnDelete(DeleteBehavior.Restrict); // منع الحذف العشوائي للحسابات المرتبطة بحركات
        }
        }
    }

