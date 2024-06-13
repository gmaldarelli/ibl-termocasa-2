using System;
using System.Collections.Generic;
using IBLTermocasa.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOFMaterials
{
    public class BillOFMaterialDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;
        public RequestForQuotationPropertyDto RequestForQuotationProperty { get; set; } = new();
        public List<BOMItemDto>? ListItems { get; set; } = new();

        public string ConcurrencyStamp { get; set; } = null!;

    }
}