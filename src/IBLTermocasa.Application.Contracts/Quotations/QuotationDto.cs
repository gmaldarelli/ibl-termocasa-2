using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public decimal? Discount { get; set; }        
        public double? MarkUp { get; set; }
        public List<QuotationItemDto>? QuotationItems { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;

        public double TotalCost => QuotationItems?.Sum(x => x.TotalCost) ?? 0;
        public double TotalSellingPrice => QuotationItems?.Sum(x => x.FinalSellingPrice) ?? 0;
        public double TotalWorkCost => QuotationItems?.Sum(x => x.WorkCost) ?? 0;
        public double TotalMaterialCost => QuotationItems?.Sum(x => x.MaterialCost) ?? 0;
        public double FinalSellingPrice => QuotationItems?.Sum(x => x.FinalSellingPrice) ?? 0;
        public double TotalMargin => TotalSellingPrice - TotalCost;
        public double AverageDiscount => QuotationItems?.Average(x => x.Discount) ?? 0;
        public double AverageMarkUp => QuotationItems?.Average(x => x.MarkUp) ?? 0;
        
    }
}