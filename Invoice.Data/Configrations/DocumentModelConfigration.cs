using Invoice.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Data.Configrations
{
    internal class DocumentModelConfigration : IEntityTypeConfiguration<DocumentModel>
    {
        public void Configure(EntityTypeBuilder<DocumentModel> builder)
        {
            // Table Name
            builder.ToTable("Documents");

            // Key
            builder.HasKey(d => d.Id);

            // Properties
            builder.Property(d => d.InternalId)
                   .IsRequired()
                   .HasMaxLength(50);

            //builder.Property(d => d.TypeName)
            //       .HasMaxLength(5);

            builder.Property(d => d.TypeVersionName)
                   .HasMaxLength(5);

            builder.Property(d => d.Status)
                   .HasMaxLength(50);

            builder.Property(d => d.TotalSales)
                   .HasColumnType("decimal(18,2)");

            builder.Property(d => d.NetAmount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(d => d.Total)
                   .HasColumnType("decimal(18,2)");
            // تجاهل خاصية document من التعيين في قاعدة البيانات
            builder.Ignore(d => d.document);

            builder.Property(d => d.ExtraDiscountAmount)
                   .HasColumnType("decimal(8,2)")
                   .HasDefaultValue(0m);

            // علاقات Parties
            builder.HasOne(d => d.Issuer)
                   .WithMany()
                   .HasForeignKey(d => d.IssuerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false); // nullable foreign key

            builder.HasOne(d => d.Receiver)
                   .WithMany()
                   .HasForeignKey(d => d.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict) 
                  .IsRequired(false);  // nullable foreign key

            // علاقات InvoiceLines
            builder.HasMany(d => d.InvoiceLines)
                   .WithOne(il => il.DocumentModel)
                   .HasForeignKey(il => il.Id);

            // علاقات TaxTotals
            builder.HasMany(d => d.TaxTotals)
                   .WithOne(tt => tt.DocumentModel)
                   .HasForeignKey(tt => tt.Id);
            // علاقه مع  category Expense
            builder.HasOne(d => d.ExpenseCategory)
                   .WithMany(ec => ec.Documents)
                   .HasForeignKey(d => d.ExpenseCategoryId)
                   .OnDelete(DeleteBehavior.SetNull)
                   .IsRequired(false); // nullable foreign key

          
        }
    }

}
