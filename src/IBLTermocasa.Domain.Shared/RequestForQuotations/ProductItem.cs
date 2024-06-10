using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.RequestForQuotations;

public class ProductItem : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public int Order { get; set; }
    public string ProductName { get; set; }
    public Guid? ParentId { get; set; }
    public List<Answer> Answers { get; set; } = new();
}