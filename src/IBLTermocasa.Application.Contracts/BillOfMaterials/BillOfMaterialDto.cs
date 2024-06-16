using System;
using System.Collections.Generic;
using IBLTermocasa.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOfMaterials
{
    public class BillOfMaterialDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string BomNumber { get; set; } = null!;
        public RequestForQuotationPropertyDto RequestForQuotationProperty { get; set; } = new();
        public List<BomItemDto>? ListItems { get; set; } = new();

        public string ConcurrencyStamp { get; set; } = null!;

    }
}