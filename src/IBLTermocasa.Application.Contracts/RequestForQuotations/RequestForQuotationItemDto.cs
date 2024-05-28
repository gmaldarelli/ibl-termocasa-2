using System;
using System.Collections.Generic;

namespace IBLTermocasa.RequestForQuotations;

public class RequestForQuotationItemDto
{
    public Guid ProductId { get; set; }
    public List<Answer> Answers { get; set; } = new();
    public string? ProductName { get; set; }
    public int Quantity { get; set; } = 1;

    public RequestForQuotationItemDto(Guid productId, List<Answer> answers, int quantity)
    {
        ProductId = productId;
        Answers = answers;
        Quantity = quantity;
    }

    public RequestForQuotationItemDto()
    {
    }
}