using Invoice.Core.Model;
using NPOI.XSSF.UserModel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
namespace Invoice.UI.Services
{
        public class JournalSuplierPDF
        {
            public void ExportSupplierJournal(string supplierName, IEnumerable<SupplierJournalEntry> entries)
            {
                if (string.IsNullOrWhiteSpace(supplierName) || entries == null || !entries.Any())
                {
                    MessageBox.Show("⚠️ لا توجد بيانات لتصديرها.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var list = entries.ToList();
                var fileName = $"{supplierName}_Journal_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            // 🔹 نحسب الإجماليات العامة:
            var nonReturnEntries = list
.Where(e => e.Description != "مردود مشتريات")
.ToList();
            decimal totalInvoiceValue = nonReturnEntries.Sum(e => e.CreditAmount);
            // ✅ الضرائب
            decimal totalTax1 = nonReturnEntries
                .SelectMany(e => e.TaxDetailsList)
                .Where(t => t.Rate == 1)
                .Sum(t => t.Amount);

            decimal totalTax14 = nonReturnEntries
                .SelectMany(e => e.TaxDetailsList)
                .Where(t => t.Rate == 14)
                .Sum(t => t.Amount);

            decimal totalSales = nonReturnEntries.Sum(e => e.Credit);
                decimal totalReceipts = nonReturnEntries.Sum(e => e.DebitAmount); // 🧾 إجمالي السداد

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

                        // 🟦 العنوان الرئيسي
                        page.Header().Element(header =>
                            header.AlignCenter().Element(container =>
                                container
                                    .PaddingBottom(10)
                                    .Text($"📒 الاســـم: {supplierName}")
                                    .FontSize(16)
                                    .Bold()
                                    .Underline()
                                    .FontColor("#1E3A8A")
                            )
                        );
                        page.Content().Column(column =>
                        {
                            // 🧾 الجدول الرئيسي
                            column.Item().AlignRight().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // الرصيد
                                    columns.RelativeColumn(3); // دائن
                                    columns.RelativeColumn(2); //مدين
                                    columns.RelativeColumn(2); // قيمة الضريبة
                                    columns.RelativeColumn(2); // نسبة الضريبة
                                    columns.RelativeColumn(3); // قيمة الفاتورة
                                    columns.RelativeColumn(2); // الوصف
                                    columns.RelativeColumn(2); // رقم الفاتورة
                                    columns.RelativeColumn(3); // التاريخ
                                });

                                // 🟩 ترويسة الأعمدة
                                string[] headers =
                                {
         "🧾 الرصيد",
         "💵 دائن",
         "💵 مدين",
         "💸 قيمة الضريبة",
         "📊 نسبة الضريبة",
         "💰 قيمة الفاتورة",
         "📘 الوصف",
         "📄 رقم الفاتورة",
         "📅 التاريخ"
     };

                                foreach (var title in headers)
                                    table.Cell().Element(CellHeaderStyle).Text(title).Bold().FontSize(10);

                                int index = 0;
                                foreach (var entry in list)
                                {
                                    var taxes = entry.TaxDetailsList?.ToList() ?? new List<TaxDetail>();
                                    if (taxes.Count == 0)
                                        taxes.Add(new TaxDetail { Rate = 0, Amount = 0 }); // حتى لا ينهار الجدول

                                    for (int t = 0; t < taxes.Count; t++)
                                    {
                                        bool isFirstRow = (t == 0);
                                        bool isEven = index % 2 == 0;
                                        string bgColor = isEven ? "#FFFFFF" : "#F9FAFB";

                                        var tax = taxes[t];

                                        // 🧾 الأعمدة المشتركة (تُملأ فقط في الصف الأول)
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstRow ? entry.Balance.ToString("N2") : "");
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstRow ? entry.CreditAmount.ToString("N2") : "");
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstRow ? entry.DebitAmount.ToString("N2") : "");

                                        // 💸 أعمدة الضريبة (تتكرر حسب عدد الضرائب)
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(tax.Amount != 0 ? tax.Amount.ToString("N2") : "-");
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(tax.Rate != 0 ? $"{tax.Rate:N2}%" : "-");

                                        // 💰 باقي الأعمدة (تُملأ فقط في الصف الأول)
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstRow ? entry.Credit.ToString("N2") : "");
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstRow ? entry.Description ?? "" : "");
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstRow ? entry.InvoiceNumber ?? "-" : "");
                                        table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstRow ? entry.Date.ToString("yyyy/MM/dd") : "");
                                    }

                                    index++;
                                }

                            });
                            // ✅ إجمالي المردودات
                            decimal totalReturns = list
                                .Where(e => e.Description == "مردود مشتريات")
                                .Sum(e => e.DebitAmount);

                            // ✅ الرصيد النهائي (آخر صف في اليومية)
                            decimal finalBalance = list.Any() ? list.Last().Balance : 0m;


                            // 🔻 قسم الإجماليات داخل نفس الـ Content
                            column.Item().PaddingTop(25).BorderTop(1).BorderColor("#9CA3AF").Table(summary =>
                            {
                                summary.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                summary.Cell().Element(CellHeaderStyle).Text("🏁 الرصيد النهائي").Bold();
                                summary.Cell().Element(CellHeaderStyle).Text("↩️ إجمالي المردودات").Bold();
                                summary.Cell().Element(CellHeaderStyle).Text("💵 إجمالي السداد").Bold();
                                summary.Cell().Element(CellHeaderStyle).Text("💳 إجمالي الفواتير").Bold();
                                summary.Cell().Element(CellHeaderStyle).Text("🧾 ضريبة 1%").Bold();
                                summary.Cell().Element(CellHeaderStyle).Text("🧾 ضريبة 14%").Bold();
                                summary.Cell().Element(CellHeaderStyle).Text("💰 إجمالي المشتريات").Bold();

                                summary.Cell().Element(c => CellStyle(c, "#ECFDF5")).Text(finalBalance.ToString("N2")); // 🟢 لون خفيف للرصيد
                                summary.Cell().Element(c => CellStyle(c, "#FEF2F2")).Text(totalReturns.ToString("N2")); // 🔴 لون خفيف للمردودات
                                summary.Cell().Element(c => CellStyle(c, "#F9FAFB")).Text(totalReceipts.ToString("N2"));
                                summary.Cell().Element(c => CellStyle(c, "#F9FAFB")).Text(totalInvoiceValue.ToString("N2"));
                                summary.Cell().Element(c => CellStyle(c, "#F9FAFB")).Text(totalTax1.ToString("N2"));
                                summary.Cell().Element(c => CellStyle(c, "#F9FAFB")).Text(totalTax14.ToString("N2"));
                                summary.Cell().Element(c => CellStyle(c, "#F9FAFB")).Text(totalSales.ToString("N2"));
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

                MessageBox.Show($"✅ تم حفظ اليومية بنجاح على سطح المكتب:\n{fileName}",
                    "تم الحفظ", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // 🎨 أنماط الخلايا
            private IContainer CellStyle(IContainer container, string bgColor)
            {
                return container
                    .Background(bgColor)
                    .PaddingVertical(2)
                    .PaddingHorizontal(4)
                    .AlignMiddle();
            }

            private IContainer CellHeaderStyle(IContainer container)
            {
                return container
                    .Background("#E5E7EB")
                    .PaddingVertical(2)
                    .PaddingHorizontal(4)
                    .AlignMiddle();
            }
        }
    }
