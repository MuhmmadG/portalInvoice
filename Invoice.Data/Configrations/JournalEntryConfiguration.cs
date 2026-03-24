using Invoice.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Data.Configrations
{
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            // Table
            builder.ToTable("JournalEntries");

            // Key
            builder.HasKey(j => j.Id);

            // Properties
            builder.Property(j => j.Date)
                   .IsRequired();

            builder.Property(j => j.Description)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            // Relationship with DocumentModel (optional)
            builder.HasOne(j => j.DocumentModel)
                   .WithMany()
                   .HasForeignKey(j => j.DocumentModelId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Lines
            builder.HasMany(j => j.Lines)
                   .WithOne(l => l.JournalEntry)
                   .HasForeignKey(l => l.JournalEntryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(j => j.Date).HasDatabaseName("IX_JournalEntries_Date");
        }
    }
}
