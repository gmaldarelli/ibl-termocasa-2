using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.RequestForQuotations;

public class RequestForQuotationItemDto : EntityDto<Guid>
{
    public int Order { get; set; } = 1;
    public int Quantity { get; set; } = 1;
    public List<ProductItemDto>? ProductItems { get; set; } = new();
    
    public RequestForQuotationItemDto(int order, int quantity, List<ProductItemDto> productItems)
    {
        Order = order;
        Quantity = quantity;
        ProductItems = productItems;
    }

    public RequestForQuotationItemDto()
    {
    }
}