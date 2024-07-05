using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationCreateDto
    {
        public string? ConsumptionProduct { get; set; }
        public string? ConsumptionWork { get; set; }
    }
}