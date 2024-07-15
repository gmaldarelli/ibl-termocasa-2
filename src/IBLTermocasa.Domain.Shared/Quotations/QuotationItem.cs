using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.RequestForQuotations;

public class QuotationItem : Entity<Guid>
{
    public Guid RFQItemId { get; set; }
    public Guid BOMItemId { get; set; }
    public Guid ProductId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public double WorkCost { get; set; }
    public double MaterialCost { get; set; }
    public double TotalCost { get; set; }
    public double SellingPrice { get; set; }
    public double MarkUp { get; set; }
    public double Discount { get; set; }
    public double FinalSellingPrice { get; set; }
    public int Quantity { get; set; }

    public QuotationItem(Guid id, Guid rfqItemId, Guid bomItemId, Guid productId, string code, string name, double workCost, double materialCost, double totalCost, double sellingPrice, double markUp, double discount, double finalSellingPrice, int quantity) : base(id)
    {
        RFQItemId = rfqItemId;
        BOMItemId = bomItemId;
        ProductId = productId;
        Code = code;
        Name = name;
        WorkCost = workCost;
        MaterialCost = materialCost;
        TotalCost = totalCost;
        SellingPrice = sellingPrice;
        MarkUp = markUp;
        Discount = discount;
        FinalSellingPrice = finalSellingPrice;
        Quantity = quantity;
    }

    public QuotationItem()
    {
    }
}