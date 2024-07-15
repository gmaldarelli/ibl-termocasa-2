using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOfMaterials;

public class BomItem : Entity<Guid>
{
    public Guid RequestForQuotationItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public List<BomProductItem> BomProductItems { get; set; } = new();

    protected BomItem()
    {
    }
    public BomItem(Guid id, Guid requestForQuotationItemId, int quantity,  List<BomProductItem>? bomProductItems) 
    {
        Id = id;
        RequestForQuotationItemId = requestForQuotationItemId;
        Quantity = quantity;
        BomProductItems = bomProductItems;
    }
}