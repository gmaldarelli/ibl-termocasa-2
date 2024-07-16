using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using IBLTermocasa.RequestForQuotations;
using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Quotations
{
    public class QuotationDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public Guid IdRFQ { get; set; }
        public Guid IdBOM { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime QuotationValidDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public QuotationStatus Status { get; set; }
        public bool DepositRequired { get; set; }
        public double? DepositRequiredValue { get; set; }
        public List<QuotationItem>? QuotationItems { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;

    }
}