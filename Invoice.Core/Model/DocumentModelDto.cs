using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace Invoice.Core.Model
{
    [XmlRoot("document")]
    public class DocumentModelDto :INotifyPropertyChanged
    {
      
        
        private int? _expenseCategoryId;
        public int? ExpenseCategoryId
        {
            get => _expenseCategoryId;
            set
            {
                if (_expenseCategoryId != value)
                {
                    _expenseCategoryId = value;
                    OnPropertyChanged(nameof(ExpenseCategoryId));
                }
            }
        }
        public DocumentModelDto()
        {
            ImportedItem = new ImportedItemDto(); // 👈 دايما موجود
        }
        [XmlIgnore] // 👈 علشان ما يعملش مشاكل مع XmlSerializer
        public ExpenseCategory ExpenseCategory { get; set; }
        public int? ImportedItemId { get; set; }
        // كود الصنف المستورد
        [XmlIgnore]   // EF Navigation - ما يتسجلش في XML
        public ImportedItemDto ImportedItem { get; set; }
       // public ImportedItemDto ImportedItem { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
   
        public decimal TotalAmount
        {
            get
            {
                if (InvoiceLines == null || !InvoiceLines.Any())
                    return 0;

                return InvoiceLines.Sum(line => line.Total);
            }
        }
        public string TaxTotalsString
        {
            get
            {
                if (TaxTotals == null || !TaxTotals.Any())
                    return string.Empty;

                return string.Join(" | ",
                    TaxTotals.Select(t => $"{t.TaxType}:{t.Amount}"));
            }
        }
        [XmlElement("uuid")]
        public string Uuid { get; set; }

        [XmlElement("submissionUUID")]
        public string SubmissionUUID { get; set; }

        [XmlElement("longId")]
        public string LongId { get; set; }
        [XmlElement("internalId")]
        public string InternalId { get; set; }

        [XmlElement("typeName")]
        public string TypeName { get; set; }

        [XmlElement("issuerId")]
        public string IssuerId { get; set; }

        [XmlElement("issuerName")]
        public string IssuerName { get; set; }

        [XmlElement("receiverId")]
        public string ReceiverId { get; set; }

        [XmlElement("receiverName")]
        public string ReceiverName { get; set; }

        [XmlElement("dateTimeIssued")]
        public DateTime DateTimeIssued { get; set; }

        [XmlElement("dateTimeReceived")]
        public DateTime DateTimeReceived { get; set; }

        [XmlElement("totalSales")]
        public decimal TotalSales { get; set; }

        [XmlElement("totalDiscount")]
        public decimal TotalDiscount { get; set; }

      
        [XmlElement("netAmount")]
        public decimal NetAmount { get; set; }

        [XmlElement("extraDiscountAmount")]
        public decimal ExtraDiscountAmount { get; set; }
        [XmlElement("total")]
        public decimal Total { get; set; }

        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("document")]
        public string document { get; set; }

        [XmlElement("issuer")]
        public IssuerDto Issuer { get; set; }

        [XmlElement("receiver")]
        public ReceiverDto Receiver { get; set; }

        [XmlArray("invoiceLines")]
        [XmlArrayItem("invoiceLine")]
        public List<InvoiceLineDto> InvoiceLines { get; set; }

        [XmlArray("taxTotals")]
        [XmlArrayItem("taxTotal")]
        public List<TaxTotalDto> TaxTotals { get; set; }

        // ---- Inner DTOs ----
        public class IssuerDto
        {
            [XmlElement("id")]
            public string Id { get; set; }

            [XmlElement("name")]
            public string Name { get; set; }

            [XmlElement("type")]
            public string Type { get; set; }

            [XmlElement("issuerId")]
            public string IssuerId { get; set; }

            [XmlElement("issuerName")]
            public string IssuerName { get; set; }

            [XmlElement("address")]
            public AddressDto Address { get; set; }
        }

        public class ReceiverDto
        {
            [XmlElement("type")]
            public string Type { get; set; }

            [XmlElement("id")]
            public string Id { get; set; }

            [XmlElement("name")]
            public string Name { get; set; }

            [XmlElement("address")]
            public AddressDto Address { get; set; }
        }

        public class AddressDto
        {
            [XmlElement("buildingNumber")]
            public string BuildingNumber { get; set; }

            [XmlElement("street")]
            public string Street { get; set; }

            [XmlElement("governate")]
            public string Governate { get; set; }

            [XmlElement("regionCity")]
            public string RegionCity { get; set; }

            [XmlElement("country")]
            public string Country { get; set; }

            [XmlElement("branchID")]
            public string BranchID { get; set; }
        }

        public class InvoiceLineDto
        {
            [XmlElement("description")]
            public string Description { get; set; }

            [XmlElement("itemType")]
            public string ItemType { get; set; }

            [XmlElement("itemCode")]
            public string ItemCode { get; set; }

            [XmlElement("unitType")]
            public string UnitType { get; set; }

            [XmlElement("quantity")]
            public decimal Quantity { get; set; }

            [XmlElement("unitValue")]
            public UnitValueDto UnitValue { get; set; }

            [XmlElement("salesTotal")]
            public decimal SalesTotal { get; set; }

            [XmlArray("taxableItems")]
            [XmlArrayItem("taxableItem")]
            public List<TaxableItemDto> TaxableItems { get; set; }

            [XmlElement("netTotal")]
            public decimal NetTotal { get; set; }

            [XmlElement("totalTaxableFees")]
            public decimal TotalTaxableFees { get; set; }

            [XmlElement("valueDifference")]
            public decimal ValueDifference { get; set; }

            [XmlElement("total")]
            public decimal Total { get; set; }

            public string TaxableItemsString
            {
                get
                {
                    if (TaxableItems == null || !TaxableItems.Any())
                        return string.Empty;

                    return string.Join(" | ",
                        TaxableItems.Select(t => $"{t.Amount}-{t.TaxType}-{t.Rate}(:Rate:%)"));
                }
            }

        }

        public class UnitValueDto
        {
            [XmlElement("currencySold")]
            public string CurrencySold { get; set; }

            [XmlElement("amountSold")]
            public decimal AmountSold { get; set; }

            [XmlElement("amountEGP")]
            public decimal AmountEGP { get; set; }

            [XmlElement("currencyExchangeRate")]
            public decimal CurrencyExchangeRate { get; set; }
        }

        public class TaxableItemDto
        {
            [XmlElement("taxType")]
            public string TaxType { get; set; }

            [XmlElement("amount")]
            public decimal Amount { get; set; }

            [XmlElement("subType")]
            public string SubType { get; set; }

            [XmlElement("rate")]
            public decimal Rate { get; set; }
        }

        public class TaxTotalDto
        {
            [XmlElement("taxType")]
            public string TaxType { get; set; }

            [XmlElement("amount")]
            public decimal Amount { get; set; }
        }

    }


}
