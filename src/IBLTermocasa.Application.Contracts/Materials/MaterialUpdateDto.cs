using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Materials
{
    public class MaterialUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public MeasureUnit MeasureUnit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Lifo { get; set; }
        public decimal StandardPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal LastPrice { get; set; }
        public decimal AveragePriceSecond { get; set; }
        public SourceType SourceType { get; set; }
        public DateTime? FirstSync { get; set; }
        public DateTime? LastSync { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}