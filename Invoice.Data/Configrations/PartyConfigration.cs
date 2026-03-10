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
    public class PartyConfigration : IEntityTypeConfiguration<Party>
    {
        public void Configure(EntityTypeBuilder<Party> builder)
        {
            // Table Name
            builder.ToTable("Parties");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.PartyType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.TaxNumber)
                   .HasMaxLength(50);

            // Relationships

            // Party -> IssuedDocuments (One-to-Many)
            builder.HasMany(p => p.IssuedDocuments)
                   .WithOne(d => d.Issuer)           // لازم يكون عندك Issuer navigation في DocumentModel
                   .HasForeignKey(d => d.IssuerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Party -> ReceivedDocuments (One-to-Many)
            builder.HasMany(p => p.ReceivedDocuments)
                   .WithOne(d => d.Receiver)         // لازم يكون عندك Receiver navigation في DocumentModel
                   .HasForeignKey(d => d.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Party -> Address (One-to-One)
            builder.HasOne(p => p.Address)
                   .WithOne(a => a.Party)            // لازم يكون عندك Party navigation في Address
                   .HasForeignKey<Address>(a => a.PartyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

