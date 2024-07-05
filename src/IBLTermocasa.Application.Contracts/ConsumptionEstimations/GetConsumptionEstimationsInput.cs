using Volo.Abp.Application.Dtos;
using System;
using System.Collections.Generic;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class GetConsumptionEstimationsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public Guid? IdProduct { get; set; }

        public GetConsumptionEstimationsInput()
        {

        }
    }
}