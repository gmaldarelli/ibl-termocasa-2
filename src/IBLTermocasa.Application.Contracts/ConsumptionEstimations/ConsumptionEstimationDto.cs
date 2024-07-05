using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid IdProduct { get; set; }
        public List<ConsumptionProductDto> ConsumptionProduct { get; set; } = new();
        public List<ConsumptionWorkDto> ConsumptionWork { get; set; } = new();
        public string ConcurrencyStamp { get; set; }
        
        public ConsumptionEstimationDto()
        {
            
        }
        
        public ConsumptionEstimationDto(Guid id, Guid idProduct, List<ConsumptionProductDto> consumptionProduct = null, List<ConsumptionWorkDto> consumptionWork = null)
        {
            Id = id;
            IdProduct = idProduct;
            ConsumptionProduct = consumptionProduct;
            ConsumptionWork = consumptionWork;
        }

    }
}