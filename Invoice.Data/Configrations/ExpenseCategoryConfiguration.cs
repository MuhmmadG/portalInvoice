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
  
        public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
        {
            public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
            {
                // تحديد اسم الجدول
                builder.ToTable("ExpenseCategories");

                // المفتاح الأساسي
                builder.HasKey(ec => ec.Id);

                // خصائص
                builder.Property(ec => ec.CategoryType)
                       .IsRequired()
                       .HasMaxLength(100);

               // العلاقة One-to-Many مع Documents
            builder.HasMany(ec => ec.Documents)
                       .WithOne(il => il.ExpenseCategory)
                       .HasForeignKey(il => il.ExpenseCategoryId)
                       .OnDelete(DeleteBehavior.Restrict);
            // 👆 Restrict عشان ميتمسحش الـ InvoiceLine بالخطأ لو اتشال التصنيف
            // ✅ Seed Data أساسية
            builder.HasData(
                new ExpenseCategory { Id = 1, CategoryType = "مصروفات عمومية" },
                new ExpenseCategory { Id = 2, CategoryType = "مصروفات تشغيلية" },
                new ExpenseCategory { Id = 3, CategoryType = "رسوم" },
                new ExpenseCategory { Id = 4, CategoryType = "تأمينات" },
                new ExpenseCategory { Id = 5, CategoryType = "مشتريات" },
                new ExpenseCategory { Id = 6, CategoryType = "م.خارجيه" },
                new ExpenseCategory { Id = 7, CategoryType = "م.تسوقيه" },
                new ExpenseCategory { Id = 8, CategoryType = "أخرى" },
                new ExpenseCategory { Id = 9, CategoryType = "أصول" },
                new ExpenseCategory { Id = 10, CategoryType = "صندوق الطوارئ" },
                new ExpenseCategory { Id = 11, CategoryType = "م.مبيعات" }
            );
        }
        }

    }


