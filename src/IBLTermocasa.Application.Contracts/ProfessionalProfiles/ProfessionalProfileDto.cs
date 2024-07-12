using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ProfessionalProfiles
{
    public class ProfessionalProfileDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public double StandardPrice { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;

    }
}