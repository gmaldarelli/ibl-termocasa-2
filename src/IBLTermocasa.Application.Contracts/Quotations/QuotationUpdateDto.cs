using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Quotations
{
    public class QuotationUpdateDto : IHasConcurrencyStamp
    {
        public Guid? TenantId { get; set; }
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
        public QuotationStatus Status { get; set; }
        public decimal? Discount { get; set; }        
        public double? MarkUp { get; set; }
        public List<double> MarkUps { get; set; } = new List<double>(){0,0,0};
        public bool DepositRequired { get; set; }
        public double? DepositRequiredValue { get; set; }
        public List<QuotationItemDto>? QuotationItems { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;
    }
}