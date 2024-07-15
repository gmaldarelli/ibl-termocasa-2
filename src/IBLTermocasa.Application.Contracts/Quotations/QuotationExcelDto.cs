using IBLTermocasa.Types;
using System;

namespace IBLTermocasa.Quotations
{
    public class QuotationExcelDto
    {
        public string IdRFQ { get; set; } = null!;
        public string IdBOM { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime SentDate { get; set; }
        public DateTime QuotationValidDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public QuotationStatus Status { get; set; }
        public bool DepositRequired { get; set; }
        public double? DepositRequiredValue { get; set; }
        public string? QuotationItems { get; set; }
    }
}