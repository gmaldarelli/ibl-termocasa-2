using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? ConsumptionProduct { get; set; }
        public string? ConsumptionWork { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}