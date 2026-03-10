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
   
        public class ItemCodeConfigConfiguration : IEntityTypeConfiguration<ItemCodeConfig>
        {
            public void Configure(EntityTypeBuilder<ItemCodeConfig> builder)
            {
                // اسم الجدول
                builder.ToTable("ItemMappings");

                // المفتاح الأساسي
                builder.HasKey(x => x.Id);

                // TaxCode مطلوب + طول أقصى
                builder.Property(x => x.TaxCode)
                       .IsRequired()
                       .HasMaxLength(50);

                // InternalCode مطلوب + طول أقصى
                builder.Property(x => x.InternalCode)
                       .IsRequired()
                       .HasMaxLength(50);

                // Unique Constraint على TaxCode
                builder.HasIndex(x => x.TaxCode)
                       .IsUnique();

            // Kind اختياري
            builder.Property(x => x.Kind)
                   .HasMaxLength(50);
                     

            // العلاقة One → Many
            builder.HasMany(x => x.InvoiceLines)
                       .WithOne(l => l.ItemMapping)
                       .HasForeignKey(l => l.ItemMappingId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

    }


