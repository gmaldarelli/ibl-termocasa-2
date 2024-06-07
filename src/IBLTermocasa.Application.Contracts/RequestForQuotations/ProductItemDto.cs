using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.RequestForQuotations;

public class ProductItemDto : EntityDto<Guid>
{ 
    public Guid ProductId { get; set; }
    public int Order { get; set; } = 1;
    public string ProductName { get; set; }
    public Guid? ParentId { get; set; }
    public List<AnswerDto> Answers { get; set; } = new();
}