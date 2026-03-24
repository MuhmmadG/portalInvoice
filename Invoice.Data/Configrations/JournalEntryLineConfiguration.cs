using Invoice.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Data.Configrations
{
    public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
    {
        public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
        {
            builder.ToTable("JournalEntryLines");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Debit)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(l => l.Credit)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
            builder.Property(l => l.Description)
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.HasOne(l => l.JournalEntry)
                   .WithMany(j => j.Lines)
                   .HasForeignKey(l => l.JournalEntryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.Account)
                   .WithMany() 
                   .HasForeignKey(l => l.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(l => l.JournalEntryId).HasDatabaseName("IX_JournalEntryLines_JournalEntryId");
        }
    }
}
