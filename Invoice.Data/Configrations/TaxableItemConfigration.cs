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
    public class TaxableItemConfigration : IEntityTypeConfiguration<TaxableItem>
    {
        public void Configure(EntityTypeBuilder<TaxableItem> builder)
        {
            // Table name
            builder.ToTable("TaxableItems");

            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.TaxType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.SubType)
                   .HasMaxLength(50);

            builder.Property(t => t.Rate)
                   .HasColumnType("decimal(5,2)") // مثال: نسبة مئوية
                   .IsRequired();

            // Relationships
            builder.HasOne(t => t.InvoiceLine)
                   .WithMany(i => i.TaxableItems)   // لازم تكون موجودة في InvoiceLine
                   .HasForeignKey(t => t.InvoiceLineId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
