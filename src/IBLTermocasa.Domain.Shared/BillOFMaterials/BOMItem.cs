using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOfMaterials;

public class BOMItem : Entity<Guid>
{
    public Guid RequestForQuotationItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public List<BOMProductItem> BomProductItems { get; set; } = new();
}