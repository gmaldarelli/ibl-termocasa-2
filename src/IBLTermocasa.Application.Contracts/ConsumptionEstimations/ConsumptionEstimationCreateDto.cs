using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationCreateDto
    {
        public Guid IdProduct { get; set; }
        public List<ConsumptionProductDto> ConsumptionProduct { get; set; } = new();
        public List<ConsumptionWorkDto> ConsumptionWork { get; set; } = new();
    }
}