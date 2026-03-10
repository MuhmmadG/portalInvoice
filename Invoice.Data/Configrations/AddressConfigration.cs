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
    public class AddressConfigration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            // اسم الجدول
            builder.ToTable("Addresses");

            // المفتاح الأساسي
            builder.HasKey(a => a.Id);

            // الخصائص
            builder.Property(a => a.buildingNumber)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(a => a.street)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(a => a.governate)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(a => a.regionCity)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(a => a.country)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(a => a.branchID)
                .HasMaxLength(50)
                .IsRequired(false);

            // العلاقة مع Party (One-to-One)
            builder.HasOne(a => a.Party)
                   .WithOne(p => p.Address)
                   .HasForeignKey<Address>(a => a.PartyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
   

