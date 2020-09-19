using System;
using System.Collections.Generic;

namespace multi_reader
{
    public class FileFormat
    {
        public string FileVersion { get; set; }
        public RawInvoice[] RawInvoices { get; set; }
        public LedgerCollection Ledgers { get; set; }
    }

    public class LedgerCollection
    {
        public Person[] People { get; set; }
        public StockItem[] StockItems { get; set; }
    }

    public class RawInvoice : InvoiceBase
    {
        public string Description { get; set; }
        public double Confidence { get; set; }
        public bool? IsProcessed { get; set; }
        public bool? IsIgnore { get; set; }

        public List<RawInvoiceItem> RawInvoiceItems { get; set; } = new List<RawInvoiceItem>();
    }

    public class RawInvoiceItem : InvoiceItemBase
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public double Confidence { get; set; }
        public bool? IsProcessed { get; set; }
        public bool? IsIgnore { get; set; }
        public object Tag { get; set; }
    }

    public class InvoiceBase : IIdentity
    {
        public string Id { get; set; }
        public string IdentityText => Number;
        public DateTime Date { get; set; }
        public string Gstin { get; set; }
        public double GrossTotal { get; set; }
        public string Number { get; set; }
        public int? POS { get; set; }
        public string ECommerceGstin { get; set; }
        public bool? IsReverseCharge { get; set; } = null;
        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public TransactionType TransactionType { get; set; }
        public TransactionMethod TransactionMethod { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string PartyName { get; set; }
        public TransactionNature Nature { get; set; }
        public bool? IsUnregistered { get; set; }

        public bool IsForeignTransaction { get; set; }
        public bool IsCreditOrDebitNote { get; set; }

        // public ForeignInvoice ForeignInvoice { get; set; }
        public CreditOrDebitNoteInvoice CreditOrDebitNoteInvoice { get; set; }

        public bool IsEdited { get; set; }
    }

    public class Invoice : InvoiceBase
    {
        public RawInvoice Raw { get; set; }
    }

    public class CreditOrDebitNoteInvoice : Invoice
    {
        public string OriginalInvoiceNumber { get; set; }
        public DateTime OriginalInvoiceDate { get; set; }
        public NoteDocumentType NoteDocumentType { get; set; }
        public NoteIssueReason NoteIssueReason { get; set; }
        public bool IsPreGst { get; set; }
    }

    public enum NoteIssueReason
    {
        SalesReturn,
        PostSaleDiscount,
        DefienciencyInService,
        CorrectionInInvoice,
        ChangeInPOS,
        FinalizationOfProvisonalAssessment,
        Others
    }

    public enum NoteDocumentType
    {
        CreditNote,
        DebitNode,
        Rejected
    }

    public interface IIdentity
    {
        string Id { get; }
        string IdentityText { get; }
    }

    public class InvoiceItemBase
    {
        public string Name { get; set; }
        public string Hsn { get; set; }
        public string Sac { get; set; }
        public string Category { get; set; }
        public double? Quantity { get; set; }
        public string Unit { get; set; }
        public double? Rate { get; set; }
        public double? TaxRate { get; set; }
        public double? TaxableValue { get; set; }
        public double? AmountIgst { get; set; }
        public double? AmountCgst { get; set; }
        public double? AmountSgst { get; set; }
        public double? AmountCess { get; set; }
        public bool? IsService { get; set; }
        public bool? IsStockItem { get; set; }
        public bool? IsExpenses { get; set; }

    }

    public class InvoiceItem : InvoiceItemBase
    {
        public RawInvoiceItem RawInvoiceItem { get; set; }
    }

    public enum TransactionType
    {
        Unknown,
        BusinessToBusiness,
        BusinessToConsumerLarge,
        BusinessToConsumerSmall,
        CreditOrDebitNote,
        CreditOrDebitNoteUnregistered,
        Export,
        TaxLiabilityOnAdvance,
        TaxLiabilityOnAdvanceAdjustment,
        Exempted
    }

    public enum TransactionNature
    {
        Unknown,
        Sales,
        Purchase,
        Export,
        Import,
        DebitNote,
        CreditNote,
        Payment,
        Receipt
    }

    public enum InvoiceType
    {
        Unknown,
        Regular,
        SezSupplyWithPayment,
        SezSupplyWithoutPayment,
        DeemedExp
    }

    public enum TransactionMethod
    {
        Unknown,
        ECommerce,
        Other
    }

    public class Ledger : IIdentity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IdentityText => Name;
        public bool? IsUnderReverseCharge { get; set; } = null;
        public bool IsNonGst { get; set; }


        public bool IsPerson { get; set; }
        public bool IsStockItem { get; set; }
        public bool IsServiceItem { get; set; }
        public bool IsTaxLedger { get; set; }
        public bool IsExpense { get; set; }
        public bool IsTradingLedger { get; set; }

        // public Change[] Changes { get; set; }
    }

    public class Person : Ledger
    {
        public string Gstin { get; set; }
        public string Pan { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public int? StateCode { get; set; } = null;
        public string State { get; set; }
        public string PhoneNumber { get; set; }
        public string ContractPerson { get; set; }
        public double? TaxRate { get; set; } = null;

        public bool IsSezUnit { get; set; }
        public bool IsForeignParty { get; set; }

        public Person()
        {
            IsPerson = true;
        }
    }

    public class StockItem : Ledger
    {
        public string Category { get; set; }
        public string Unit { get; set; }
        public string Hsn { get; set; }
        public double? TaxRate { get; set; } = null;

        public StockItem()
        {
            IsStockItem = true;
        }
    }
}