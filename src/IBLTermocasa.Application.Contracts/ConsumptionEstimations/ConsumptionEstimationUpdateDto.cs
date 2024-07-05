using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationUpdateDto : IHasConcurrencyStamp
    {
        public Guid IdProduct { get; set; }
        public List<ConsumptionProductDto> ConsumptionProduct { get; set; } = new();
        public List<ConsumptionWorkDto> ConsumptionWork { get; set; } = new();

        public string ConcurrencyStamp { get; set; } = null!;
    }
}