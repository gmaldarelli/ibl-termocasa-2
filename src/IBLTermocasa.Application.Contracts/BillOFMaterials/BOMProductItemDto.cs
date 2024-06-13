using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.BillOFMaterials;

public class BOMProductItemDto : EntityDto<Guid>
{
    public Guid ProductItemId { get; set; }
    public string ProductItemName { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Guid? ParentBOMProductItemId { get; set; }
    public List<BOMComponentDto> BomComponents { get; set; } = new();
}