using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class GetConsumptionEstimationsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? ConsumptionProduct { get; set; }
        public string? ConsumptionWork { get; set; }

        public GetConsumptionEstimationsInput()
        {

        }
    }
}