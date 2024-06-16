using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.BillOfMaterials;

public class BomProductItemDto : EntityDto<Guid>
{
    public Guid ProductItemId { get; set; }
    public string ProductItemName { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Guid? ParentBOMProductItemId { get; set; }
    public List<BomComponentDto> BomComponents { get; set; } = new();
}