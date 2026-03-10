using Invoice.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Data.Configrations
{
    public class ExternalExpenseConfiguration : IEntityTypeConfiguration<ExternalExpense>
    {
        public void Configure(EntityTypeBuilder<ExternalExpense> builder)
        {
            // تعيين اسم الجدول
            builder.ToTable("ExternalExpenses");

            // المفتاح الأساسي (Id) يكون دائماً NOT NULL ولا يمكن أن يكون NULL
            builder.HasKey(e => e.Id);

            // الخصائص من نوع string (Reference Types)
            builder.Property(e => e.ReleaseNumber)
                    .HasMaxLength(50); // افتراضياً يقبل NULL

            builder.Property(e => e.Description)
                    .HasMaxLength(50); // افتراضياً يقبل NULL

            builder.Property(e => e.EPaymentNumber)
                    .HasMaxLength(50); // افتراضياً يقبل NULL

            // ------------------------------------------------------------------
            // الخصائص من نوع decimal (Value Types)
            // نستخدم .IsRequired(false) لنجعل العمود في قاعدة البيانات يقبل NULL
            // ------------------------------------------------------------------

            builder.Property(e => e.Value)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false); // <--- السماح بـ NULL

            builder.Property(e => e.Vat)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false); // <--- السماح بـ NULL

            builder.Property(e => e.Total)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false); // <--- السماح بـ NULL

            builder.Property(e => e.ImportTax)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false); // <--- السماح بـ NULL

            builder.Property(e => e.Fees)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false); // <--- السماح بـ NULL

            builder.Property(e => e.ProfitTax)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false); // <--- السماح بـ NULL
        }

    }
}
