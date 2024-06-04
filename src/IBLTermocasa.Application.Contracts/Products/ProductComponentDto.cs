using System;
using IBLTermocasa.Components;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products;

public class ProductComponentDto : EntityDto<Guid>
{
    public int Order { get; set; }
    public bool Mandatory { get; set; }
    public ComponentDto Component { get;  set; }
}