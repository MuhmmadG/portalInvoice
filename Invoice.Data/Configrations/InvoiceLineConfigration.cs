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
    public class InvoiceLineConfigration : IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(EntityTypeBuilder<InvoiceLine> builder)
        {
            // Table Name
            builder.ToTable("InvoiceLines");

            // Primary Key
            builder.HasKey(i => i.Id);

            // Properties
            builder.Property(i => i.description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(i => i.itemType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.itemCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.unitType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.quantity)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.salesTotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.netTotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.totalTaxableFees)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.valueDifference)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.total)
                .HasColumnType("decimal(18,2)");

            // Relationships

            // علاقة مع DocumentModel (Many InvoiceLines -> One DocumentModel)
            builder.HasOne(i => i.DocumentModel)
                   .WithMany(d => d.InvoiceLines)
                   .HasForeignKey(i => i.DocumentModelId)
                   .OnDelete(DeleteBehavior.Cascade);

            // علاقة 1 : 1 مع UnitValue
            builder.OwnsOne(i => i.unitValue, uv =>
            {
                uv.Property(u => u.CurrencySold)
                    .HasMaxLength(10);
                uv.Property(u => u.AmountEGP)
                    .HasColumnType("decimal(18,2)");
                uv.Property(u => u.AmountSold)
                    .HasColumnType("decimal(18,2)");
                uv.Property(u => u.CurrencyExchangeRate)
                    .HasColumnType("decimal(18,4)");
            });

            // علاقة One to Many مع TaxableItem
            builder.HasMany(i => i.TaxableItems)
                   .WithOne(t => t.InvoiceLine)
                   .HasForeignKey(t => t.InvoiceLineId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
