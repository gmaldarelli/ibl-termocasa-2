using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.RequestForQuotations;

public class RequestForQuotationItem : Entity<Guid>
{
    public int Order { get; set; } = 1;
    public int Quantity { get; set; } = 1;
    public List<ProductItem> ProductItems { get; set; } = new();

    public RequestForQuotationItem(Guid id, int order, int quantity, List<ProductItem> productItems)
    {
        Id = id;
        Order = order;
        Quantity = quantity;
        ProductItems = productItems;
    }

    public RequestForQuotationItem()
    {
    }
}