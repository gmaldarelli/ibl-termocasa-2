using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using IBLTermocasa.RequestForQuotations;

namespace IBLTermocasa.Quotations
{
    public class QuotationCreateDto
    {
        [Required]
        public Guid IdRFQ { get; set; }
        [Required]
        public Guid IdBOM { get; set; }
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public DateTime SentDate { get; set; }
        public DateTime QuotationValidDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public QuotationStatus Status { get; set; } = ((QuotationStatus[])Enum.GetValues(typeof(QuotationStatus)))[0];
        public bool DepositRequired { get; set; }
        public double? DepositRequiredValue { get; set; }
        public List<QuotationItem>? QuotationItems { get; set; }
    }
}