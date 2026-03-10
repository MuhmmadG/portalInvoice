using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Invoice.Core.Model;

namespace Invoice.UI.Services
{
    public class FinancialJournalPdfService
    {
        public void ExportToPdf(IEnumerable<FinancialJournalEntry> entries, DateTime startDate, DateTime endDate, decimal totalDebit, decimal totalCredit, decimal balance)
        {
            if (entries == null || !entries.Any())
            {
                MessageBox.Show("⚠️ لا توجد بيانات لتصديرها.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var list = entries.ToList();
            var fileName = $"اليومية_المصروفات_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    // 🟦 العنوان الرئيسي
                    page.Header().Element(header =>
                        header.AlignCenter().Element(container =>
                            container
                                .PaddingBottom(10)
                                .Text($"📘 اليومية المصروفات")
                                .FontSize(16)
                                .Bold()
                                .Underline()
                        )
                    );

                    // 🧾 المحتوى
                    page.Content().Column(column =>
                    {
                        // ✅ الجدول الرئيسي
                        column.Item().AlignRight().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // الرصيد
                                columns.RelativeColumn(2); // عمولة البنك
                                columns.RelativeColumn(2); // دائن
                                columns.RelativeColumn(2); // مدين
                                columns.RelativeColumn(4); // البيان
                                columns.RelativeColumn(2); // التاريخ
                            });

                            // 🟩 رؤوس الأعمدة (من اليمين إلى اليسار)
                            string[] headers =
                            {
                                "📊 الرصيد",
                                "🏦 عمولة البنك",
                                "💵 دائن",
                                "💰 مدين",
                                "📘 البيان",
                                "📅 التاريخ"
                            };

                            foreach (var title in headers)
                                table.Cell().Element(CellHeaderStyle).Text(title).Bold().FontSize(10);

                            int index = 0;
                            foreach (var entry in list)
                            {
                                bool isEven = index % 2 == 0;
                                string bgColor = isEven ? "#FFFFFF" : "#F9FAFB";

                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(entry.Balance.ToString("N2"));
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(entry.BankCommission?.ToString("N2") ?? "-");
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(entry.Credit?.ToString("N2") ?? "");
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(entry.Debit?.ToString("N2") ?? "");
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(entry.Description ?? "");
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(entry.TransactionDate.ToString("yyyy/MM/dd"));

                                index++;
                            }
                        });

                        // 🧮 قسم الإجماليات داخل نفس الصفحة
                        column.Item().PaddingTop(25).BorderTop(1).BorderColor("#9CA3AF").Table(summary =>
                        {
                            summary.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            summary.Cell().Element(CellHeaderStyle).Text("💰 إجمالي المدين").Bold();
                            summary.Cell().Element(CellHeaderStyle).Text("💵 إجمالي الدائن").Bold();
                            summary.Cell().Element(CellHeaderStyle).Text("📊 الرصيد النهائي").Bold();

                            summary.Cell().Element(c => CellStyle(c, "#F9FAFB")).Text(totalDebit.ToString("N2"));
                            summary.Cell().Element(c => CellStyle(c, "#F9FAFB")).Text(totalCredit.ToString("N2"));
                            summary.Cell().Element(c => CellStyle(c, "#F9FAFB")).Text(balance.ToString("N2"));
                        });
                    });

                    // 🔻 التذييل
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("تاريخ الطباعة: ").SemiBold();
                        x.Span(DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
                    });
                });
            });

            document.GeneratePdf(savePath);

            MessageBox.Show($"✅ تم حفظ اليومية المالية بنجاح على سطح المكتب:\n{fileName}",
                "تم الحفظ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 🎨 أنماط الخلايا
        private IContainer CellStyle(IContainer container, string bgColor)
        {
            return container
                .Background(bgColor)
                .PaddingVertical(3)
                .PaddingHorizontal(4)
                .AlignMiddle();
        }

        private IContainer CellHeaderStyle(IContainer container)
        {
            return container
                .Background("#E5E7EB")
                .PaddingVertical(3)
                .PaddingHorizontal(4)
                .AlignMiddle();
        }
    }
}
