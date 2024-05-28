using System;
using System.Collections.Generic;

namespace IBLTermocasa.RequestForQuotations;

public class RequestForQuotationItem
{
    public Guid ProductId { get; set; }
    public List<Answer> Answers { get; set; } = new();
    public int Quantity { get; set; } = 1;

    public RequestForQuotationItem(Guid productId, List<Answer> answers, int quantity)
    {
        ProductId = productId;
        Answers = answers;
        Quantity = quantity;
    }

    public RequestForQuotationItem()
    {
    }
}