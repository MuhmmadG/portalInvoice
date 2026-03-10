using Invoice.Core.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Invoice.UI.Services
{
    public class ExcelExportService
    {
        public void ExportExternalPurchaseReport(IEnumerable<ExternalPurchaseReportModel> entries)
        {
            if (entries == null)
            {
                MessageBox.Show("لا توجد بيانات لتصديرها.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // ✅ إعداد الترخيص الجديد (EPPlus 8.x)
               // ExcelPackage.License.SetNonCommercialPersonal("Muhammed Gamal");

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("تقرير المشتريات الخارجية");

                    var headers = new[]
                    {
                        "التاريخ", "الاسم", "الرقم الضريبي", "رقم المستند", "رقم الشهادة", "البيان",
                        "القيمة", "ض.ق.م", "ض.ا.ت", "ضريبة الوارد", "الرسوم", "الإجمالي", "رقم السداد الإلكتروني"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    int row = 2;

                    foreach (var item in entries)
                    {
                        worksheet.Cells[row, 1].Value = item.ReportDate;
                        worksheet.Cells[row, 1].Style.Numberformat.Format = "yyyy/MM/dd";
                        worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        worksheet.Cells[row, 2].Value = item.Name;
                        worksheet.Cells[row, 3].Value = item.TaxNumber;
                        worksheet.Cells[row, 4].Value = item.InternalId;
                        worksheet.Cells[row, 5].Value = item.ItemDetails;
                        worksheet.Cells[row, 6].Value = item.ItemName;
                        worksheet.Cells[row, 7].Value = item.NumericValue ?? 0;
                        worksheet.Cells[row, 8].Value = item.TaxT1;
                        worksheet.Cells[row, 9].Value = item.TaxT4;
                        worksheet.Cells[row, 10].Value = item.ImportTax ?? 0;
                        worksheet.Cells[row, 11].Value = item.Fees ?? 0;
                        worksheet.Cells[row, 12].Value = item.Total ?? 0;
                        worksheet.Cells[row, 13].Value = item.EPaymentNumber;

                        // تنسيق الأعمدة الرقمية
                        for (int c = 7; c <= 12; c++)
                        {
                            worksheet.Cells[row, c].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells[row, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }

                        // ⭐ تلوين الصف إذا كان مرتجع
                        if (item.IsReturn)
                        {
                            for (int col = 1; col <= 13; col++)
                            {
                                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                worksheet.Cells[row, col].Style.Font.Bold = true; // اختياري
                            }
                        }

                        row++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    var dialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel Files (*.xlsx)|*.xlsx",
                        FileName = $"تقرير المشتريات الخارجية_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        File.WriteAllBytes(dialog.FileName, package.GetAsByteArray());
                        MessageBox.Show("تم تصدير التقرير بنجاح إلى Excel.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تصدير التقرير:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
