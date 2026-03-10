using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invoice.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Invoice.Data.Configrations
{
    public class TaxTotalConfigration : IEntityTypeConfiguration<TaxTotal>
    {
        public void Configure(EntityTypeBuilder<TaxTotal> builder)
        {
            // اسم الجدول
            builder.ToTable("TaxTotals");

            // المفتاح الأساسي
            builder.HasKey(t => t.Id);

            // الخصائص
            builder.Property(t => t.TaxType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            // العلاقة مع DocumentModel
            builder.HasOne(t => t.DocumentModel)              // TaxTotal له فاتورة واحدة
                   .WithMany(d => d.TaxTotals)                // DocumentModel عنده كذا TaxTotal
                   .HasForeignKey(t => t.DocumentModelId)     // مفتاح الربط
                   .OnDelete(DeleteBehavior.Cascade);         // عند حذف الفاتورة يحذف الـ TaxTotals
        }
    }
}

