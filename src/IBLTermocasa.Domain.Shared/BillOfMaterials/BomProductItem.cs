using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOfMaterials;

public class BomProductItem : Entity<Guid>
{
    public Guid ProductItemId { get; set; }
    public string ProductItemName { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Guid? ParentBOMProductItemId { get; set; }
    public List<BomComponent> BomComponents { get; set; } = new();
    public List<BowItem> BowItems { get; set; } = new();
    
    protected BomProductItem()
    {
    }
    public BomProductItem(Guid id, Guid productItemId, string productItemName, Guid productId, Guid? parentBomProductItemId, List<BomComponent>? bomComponents, List<BowItem>? bowItems)
    {
        Id = id;
        ProductItemId = productItemId;
        ProductItemName = productItemName;
        ProductId = productId;
        ParentBOMProductItemId = parentBomProductItemId;
        if (bomComponents != null)
        {
            BomComponents = bomComponents;
        }
        if (bowItems != null)
        {
            BowItems = bowItems;
        }
    }
}