using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Materials
{
    public abstract class MaterialUpdateDtoBase : IHasConcurrencyStamp
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

        public string ConcurrencyStamp { get; set; } = null!;
    }
}