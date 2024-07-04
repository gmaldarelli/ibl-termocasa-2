using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationUpdateDto : IHasConcurrencyStamp
    {
        public string? ConsumptionProduct { get; set; }
        public string? ConsumptionWork { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}