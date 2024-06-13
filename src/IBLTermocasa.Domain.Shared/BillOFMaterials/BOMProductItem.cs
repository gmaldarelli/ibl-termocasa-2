using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOFMaterials;

public class BOMProductItem : Entity<Guid>
{
    public Guid ProductItemId { get; set; }
    public string ProductItemName { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Guid? ParentBOMProductItemId { get; set; }
    public List<BOMComponent> BomComponents { get; set; } = new();
}