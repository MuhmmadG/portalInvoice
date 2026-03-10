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
    public class OtherExpenseConfiguration : IEntityTypeConfiguration<OtherExpense>
    {
        public void Configure(EntityTypeBuilder<OtherExpense> builder)
        {
            // اسم الجدول
            builder.ToTable("OtherExpenses");

            // المفتاح الأساسي
            builder.HasKey(e => e.Id);

            // الخصائص
            builder.Property(e => e.Date)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Value)
                   .HasColumnType("decimal(18,2)");

            builder.Property(e => e.BankCommission)
                   .HasColumnType("decimal(18,2)");

            // الخاصية المحسوبة (لن تُخزن في قاعدة البيانات)
            builder.Ignore(e => e.Total);

            // العلاقة مع ExpenseCategory
            builder.HasOne(e => e.ExpenseCategory)
                   .WithMany(c => c.OtherExpenses)
                   .HasForeignKey(e => e.ExpenseCategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // منع الحذف المتسلسل
        }
    }
}
