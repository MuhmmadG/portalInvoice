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
   
    public class ImportedItemConfiguration : IEntityTypeConfiguration<ImportedItem>
    {
        public void Configure(EntityTypeBuilder<ImportedItem> builder)
        {
            // تحديد اسم الجدول
            builder.ToTable("ImportedItems");

            // المفتاح الأساسي
            builder.HasKey(ii => ii.Id);

            // الخصائص
            builder.Property(ii => ii.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(ii => ii.Description)
                   .HasMaxLength(500);

            // العلاقة One-to-Many مع DocumentModel
            builder.HasMany(ii => ii.Documents)
                   .WithOne(d => d.ImportedItem)
                   .HasForeignKey(d => d.ImportedItemId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
