using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoice.Core.Model
{
    public enum AccountType
    {
        Asset,
        Liability,
        Equity,
        Revenue,
        Expense
    }
    public enum TransactionType
    {
        Payment = 0,  // سداد
        Receipt = 1   // تحصيل
    }
    public enum PaymentMethod
    {
        Cash = 0,
        Cheque = 1,
        BankTransfer = 2
    }
    public class DocumentModel
    {
        // ... الخصائص اللي عندك بالفعل
        public int Id { get; set; }
        public string InternalId { get; set; } // رقم الفاتورة    
        public string TypeVersionName { get; set; }

        public DateTime DateTimeIssued { get; set; }
        public DateTime DateTimeReceived { get; set; }
        public decimal TotalSales { get; set; }

        public decimal ExtraDiscountAmount { get; set; }

        public decimal NetAmount { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string document { get; set; } // يحتوي على JSON
        // ---- العلاقات ----
        public Party? Issuer { get; set; }
        public int? IssuerId { get; set; }
        public Party? Receiver { get; set; }
        public int? ReceiverId { get; set; }
        public List<InvoiceLine> InvoiceLines { get; set; }
        public ICollection<TaxTotal> TaxTotals { get; set; } = new List<TaxTotal>();
        // 👇 العلاقة مع الصنف المستورد
        public int? ImportedItemId { get; set; }
        public ImportedItem ImportedItem { get; set; }

        // 👇 العلاقة الجديدة مع المصروف

        public int? ExpenseCategoryId { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public ICollection<FinancialTransaction> FinancialTransactions { get; set; } = new List<FinancialTransaction>();
    }
    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PartyType { get; set; } // Person / Company
        public string TaxNumber { get; set; }

        // Navigation
        public List<DocumentModel> IssuedDocuments { get; set; }
        public List<DocumentModel> ReceivedDocuments { get; set; }
        public Address Address { get; set; }
        // أضف هذه المجموعة لتخزين الحركات المالية المرتبطة
        
        public ICollection<FinancialTransaction> FinancialTransactions { get; set; }

    }
    public class Issuer
    {
        int Id_PK { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string issuerId { get; set; }
        public string issuerName { get; set; }
    }
    public class Receiver
    {
        int Id_PK { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }
    public class Address
    {
        public int Id { get; set; }
        public string buildingNumber { get; set; }
        public string street { get; set; }
        public string governate { get; set; }
        public string regionCity { get; set; }
        public string country { get; set; }
        public string branchID { get; set; }
        public int PartyId { get; set; } // Foreign key for Party
        public Party Party { get; set; }
    }
    public class InvoiceLine
    {

        public int Id { get; set; }
        public string description { get; set; }
        public string itemType { get; set; }
        public string itemCode { get; set; }
        public string unitType { get; set; }
        public decimal quantity { get; set; }
        public UnitValue unitValue { get; set; }
        public decimal salesTotal { get; set; }

        public decimal netTotal { get; set; }
        public decimal totalTaxableFees { get; set; }
        public decimal valueDifference { get; set; }
        public decimal total { get; set; }
        public ICollection<TaxableItem> TaxableItems { get; set; }
        public int DocumentModelId { get; set; } // Foreign key for DocumentModel
        public DocumentModel DocumentModel { get; set; }
        // 👇 العلاقة مع ItemCodeConfig (واحد بس لكل InvoiceLine)
        public int? ItemMappingId { get; set; }
        public ItemCodeConfig ItemMapping { get; set; }
        // 👇 العلاقة الجديدة مع المصروف
        public int? ExpenseCategoryId { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }

    }
    public class UnitValue
    {
        public int Id_PK { get; set; }
        public string CurrencySold { get; set; }
        public decimal AmountSold { get; set; }
        public decimal AmountEGP { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        public int InvoiceLineId { get; set; } // Foreign key for InvoiceLine
        public InvoiceLine InvoiceLine { get; set; }
    }
    public class TaxableItem
    {
        public int Id { get; set; }
        public string TaxType { get; set; }
        public decimal Amount { get; set; }
        public string SubType { get; set; }
        public decimal Rate { get; set; }
        public int InvoiceLineId { get; set; } // Foreign Key // Foreign key for InvoiceLine


        public InvoiceLine InvoiceLine { get; set; }
    }
    public class TaxTotal
    {
        public int Id { get; set; }
        public string TaxType { get; set; }
        public decimal Amount { get; set; }
        public int DocumentModelId { get; set; } // Foreign key for DocumentModel
        public DocumentModel DocumentModel { get; set; }


        public int? AccountId { get; set; }
        public ChartOfAccount Account { get; set; }
    }
    public class ExpenseCategory
    {
        public int Id { get; set; }

        public string CategoryType { get; set; }

        // 👇 كل مصروف ممكن يرتبط بأكتر من فاتورة
        public ICollection<DocumentModel> Documents { get; set; }
        // ✨ جديد: مصروفات أخرى مرتبطة
        public ICollection<OtherExpense> OtherExpenses { get; set; }
    }
    public class ItemCodeConfig
    {
        public int Id { get; set; }
        public string? TaxCode { get; set; }
        public string? InternalCode { get; set; }
        public string? Kind { get; set; }


        // 👇 كل كود ممكن يكون ليه أكتر من سطر فاتورة
        public ICollection<InvoiceLine> InvoiceLines { get; set; }
     
    }
    public class ImportedItem
    {
        public int Id { get; set; }
        public string Name { get; set; }        // مثال: "زيت"، "سليكلور"
        public string? Description { get; set; }

        // 👇 كل صنف ممكن يبقى ليه أكتر من فاتورة خارجية
        public ICollection<DocumentModel> Documents { get; set; }
    }
    public class OtherExpense
    {
      
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        public string Description { get; set; }

        public decimal Value { get; set; }

        public decimal BankCommission { get; set; }

        public decimal Total => Value + BankCommission;

        
        public int ExpenseCategoryId { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
    }
    public class FinancialTransaction
    {
        [Key]
        public int Id { get; set; }

        // 📅 التاريخ
        public DateTime Date { get; set; } = DateTime.Now;

        // 💰 نوع الحركة (سداد / تحصيل)
        public TransactionType TransactionType { get; set; }

        // 🔗 رقم الفاتورة (اختياري)
        public int? DocumentModelId { get; set; }
        public DocumentModel DocumentModel { get; set; }

        // الحساب المحاسبي
        public int? AccountId { get; set; }
        public ChartOfAccount Account { get; set; }

        // 🏢 الطرف المالي (عميل أو مورد)
        public int? PartyId { get; set; }
        public Party Party { get; set; }

        // في حالة أن الطرف غير موجود في قاعدة البيانات
        public string ManualPartyName { get; set; }

        // 💵 المبلغ الإجمالي للحركة
        public decimal Amount { get; set; }

        // 💳 طريقة الدفع (نقدي - شيك - تحويل)
        public PaymentMethod PaymentMethod { get; set; }

        // 🧾 تفاصيل الشيك (فقط إذا كان طريقة الدفع شيك)
        public string? ChequeNumber { get; set; }
        public DateTime? DueDate { get; set; }

        // 🔁 في حالة التحويل البنكي، ممكن تضيف:
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }

        // 📝 ملاحظات إضافية
        public string? Notes { get; set; }

        // 🧮 خاصية محسوبة (غير مخزنة في DB)
        [NotMapped]
        public string PaymentMethodDisplay =>
            PaymentMethod switch
            {
                PaymentMethod.Cash => "نقدي",
                PaymentMethod.Cheque => $"شيك رقم {ChequeNumber}",
                PaymentMethod.BankTransfer => "تحويل بنكي",
                _ => "غير محدد"
            };
        public int? OtherExpenseId { get; set; }   // المفتاح الأجنبي (اختياري)
        public OtherExpense OtherExpense { get; set; }  // العلاقة

    }
    public class ChartOfAccount
    {
        public int Id { get; set; }

        public string CodeAccount { get; set; }

        public string AccountName { get; set; }

        public string FinancialStatement { get; set; }

        public int Level { get; set; }

        public AccountType AccountType { get; set; }

        // 👇 الأب
        public int? ParentAccountId { get; set; }

        public ChartOfAccount ParentAccount { get; set; }

        // 👇 الأبناء
        public ICollection<ChartOfAccount> Children { get; set; }
            = new List<ChartOfAccount>();

        // 👇 الحركات المالية
        public ICollection<FinancialTransaction> FinancialTransactions { get; set; }

        // 👇 الضرائب
        public ICollection<TaxTotal> TaxTotals { get; set; }
    }


}