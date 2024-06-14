using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.BillOfMaterials;

public class BOMItemDto: EntityDto<Guid>
{
    public Guid RequestForQuotationItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public List<BOMProductItemDto> BomProductItems { get; set; } = new();
}