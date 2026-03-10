using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using Invoice.Core.Interfaces;
using Invoice.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Invoice.Data.Services
{
    public class InvoiceReaderFile : IInvoiceReaderFile
    {
        // دالة لقراءة محتوى الملف وتحديد نوعه (JSON أو XML)
        public List<DocumentModelDto> InvoiceReaderFiles(string folderPath)
        {
            var documents = new List<DocumentModelDto>();
            documents.Clear();
            var files = Directory.GetFiles(folderPath, "*.xml");

            foreach (var file in files)
            {
                try
                {
                    // Deserialize ملف XML الأساسي
                    var serlialize = new XmlSerializer(typeof(DocumentModelDto));
                    using var stream = new FileStream(file, FileMode.Open);
                    var doc = (DocumentModelDto)serlialize.Deserialize(stream);

                    // أضف العنصر إلى القائمة
                    documents.Add(doc);
                    int currentIndex = documents.Count - 1;
                    // احصل على العنصر المضاف للتعديل عليه لاحقاً
                    var currentDoc = documents[currentIndex];
                    // لو document يحتوي على JSON
                    if (IsJson(currentDoc.document))
                    {
                        try
                        {
                            var parsedFromJson = JsonConvert.DeserializeObject<DocumentModelDto>(currentDoc.document);
                            // تعبئة الخصائص المطلوبة داخل نفس الكائن الحالي
                            currentDoc.Issuer = parsedFromJson.Issuer;
                            currentDoc.Receiver = parsedFromJson.Receiver;
                            currentDoc.InvoiceLines = parsedFromJson.InvoiceLines;
                            currentDoc.TaxTotals = parsedFromJson.TaxTotals;
                            currentDoc.ExtraDiscountAmount = parsedFromJson.ExtraDiscountAmount;
                            // ... وهكذا حسب خصائصك
                        }
                        catch (Exception jsonEx)
                        {
                            Console.WriteLine($"تحذير: فشل قراءة JSON في الملف: {file} - {jsonEx.Message}");
                        }
                    }
                    // لو document يحتوي على XML داخلي
                    else if (IsXml(currentDoc.document))
                    {
                        try
                        {
                            XDocument doc1 = XDocument.Parse(currentDoc.document);
                            // أضف هذا السطر لقراءة القيمة من الـ XML المدمج
                            currentDoc.ExtraDiscountAmount = GetDecimalValue(doc1, "extraDiscountAmount");
                            currentDoc.Issuer = new DocumentModelDto.IssuerDto
                            {
                                IssuerName = GetStringValue(doc1, "issuer/name"),
                                IssuerId = GetStringValue(doc1, "issuer/id"),
                                Type = GetStringValue(doc1, "issuer/type"),
                                Address = new DocumentModelDto.AddressDto
                                {
                                    BuildingNumber = GetStringValue(doc1, "receiver/address/buildingNumber"),
                                    Street = GetStringValue(doc1, "receiver/address/street"),
                                    Governate = GetStringValue(doc1, "receiver/address/governate"),
                                    RegionCity = GetStringValue(doc1, "receiver/address/regionCity"),
                                    Country = GetStringValue(doc1, "receiver/address/country"),
                                }
                            };
                            currentDoc.TaxTotals = new List<DocumentModelDto.TaxTotalDto>
                                 {
                        new DocumentModelDto.TaxTotalDto
                        {
                            TaxType = GetStringValue(doc1, "taxTotals/taxTotal/taxType"),
                            Amount = GetDecimalValue(doc1, "taxTotals/taxTotal/amount")
                        }
                    };

                            currentDoc.Receiver = new DocumentModelDto.ReceiverDto
                            {
                                Id = GetStringValue(doc1, "receiver/id"),
                                Name = GetStringValue(doc1, "receiver/name"),
                                Type = GetStringValue(doc1, "receiver/type"),
                                Address = new DocumentModelDto.AddressDto
                                {
                                    BuildingNumber = GetStringValue(doc1, "receiver/address/buildingNumber"),
                                    Street = GetStringValue(doc1, "receiver/address/street"),
                                    Governate = GetStringValue(doc1, "receiver/address/governate"),
                                    RegionCity = GetStringValue(doc1, "receiver/address/regionCity"),
                                    Country = GetStringValue(doc1, "receiver/address/country"),
                                }
                            };
                            currentDoc.InvoiceLines = new List<DocumentModelDto.InvoiceLineDto>
                    {
                        new DocumentModelDto.InvoiceLineDto
                        {
                            Description = GetStringValue(doc1, "invoiceLines/invoiceLine/description"),
                            ItemCode = GetStringValue(doc1, "invoiceLines/invoiceLine/itemCode"),
                            Quantity = GetDecimalValue(doc1, "invoiceLines/invoiceLine/quantity"),
                            UnitType = GetStringValue(doc1, "invoiceLines/invoiceLine/unitType"),
                            SalesTotal = GetDecimalValue(doc1, "invoiceLines/invoiceLine/salesTotal"),
                            ItemType = GetStringValue(doc1, "invoiceLines/invoiceLine/itemType"),
                            UnitValue = new DocumentModelDto.UnitValueDto
                            {
                                CurrencySold = GetStringValue(doc1, "invoiceLines/invoiceLine/unitValue/currencySold"),
                                AmountSold = GetDecimalValue(doc1, "invoiceLines/invoiceLine/unitValue/amountSold"),
                                AmountEGP = GetDecimalValue(doc1, "invoiceLines/invoiceLine/unitValue/amountEGP"),
                                CurrencyExchangeRate = GetDecimalValue(doc1, "invoiceLines/invoiceLine/unitValue/currencyExchangeRate")
                            },
                            TaxableItems = new List<DocumentModelDto.TaxableItemDto>
                            {
                                new DocumentModelDto.TaxableItemDto
                                {
                                    Rate = GetDecimalValue(doc1, "invoiceLines/invoiceLine/taxableItems/taxableItem/rate"),
                                    Amount = GetDecimalValue(doc1, "invoiceLines/invoiceLine/taxableItems/taxableItem/amount"),
                                    SubType = GetStringValue(doc1, "invoiceLines/invoiceLine/taxableItems/taxableItem/subType"),
                                    TaxType = GetStringValue(doc1, "invoiceLines/invoiceLine/taxableItems/taxableItem/taxType")
                                }
                            }
                        }
                    };

                        }
                        catch (Exception innerXmlEx)
                        {
                            Console.WriteLine($"تحذير: فشل تحليل XML داخلي في الملف: {file} - {innerXmlEx.Message}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"خطأ في قراءة ملف XML: {file} - {ex.Message}");
                }

            }
            return documents;
        }

        static bool IsJson(string input)
        {
            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}")) || // Object
                   (input.StartsWith("[") && input.EndsWith("]"));  // Array
        }
        // دالة للتحقق إذا كان النص XML
        static bool IsXml(string input)
        {
            input = input.Trim();
            return input.StartsWith("<") && input.EndsWith(">");
        }
        static decimal GetDecimalValue(XDocument doc, string path)
        {
            XElement element = doc.Root.XPathSelectElement(path);
            if (element == null || string.IsNullOrWhiteSpace(element.Value))
                return 0; // إرجاع 0 في حالة عدم وجود قيمة
            return decimal.TryParse(element.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) ? result : 0;
        }
        static string GetStringValue(XDocument doc, string path)
        {
            XElement element = doc.Root.XPathSelectElement(path);
            return element != null ? element.Value.Trim() : string.Empty;
        }

    }
}

