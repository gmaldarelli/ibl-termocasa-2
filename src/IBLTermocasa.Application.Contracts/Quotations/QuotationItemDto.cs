using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using IBLTermocasa.RequestForQuotations;
using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Quotations
{
    public class QuotationItemDto : EntityDto<Guid>
    {
        public Guid RFQItemId { get; set; }
        public Guid? BOMItemId { get; set; }
        public Guid? ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double WorkCost { get; set; }
        public double MaterialCost { get; set; }
        public double TotalCost { get; set; }
        public double SellingPrice { get; set; }
        public double MarkUp { get; set; }
        public double Discount { get; set; }
        public double FinalSellingPrice { get; set; }
        public int Quantity { get; set; }

    }
}