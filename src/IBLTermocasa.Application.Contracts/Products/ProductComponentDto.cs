using System;
using System.ComponentModel.DataAnnotations;
using IBLTermocasa.Common;
using IBLTermocasa.Components;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products;

public class ProductComponentDto : EntityDto<Guid>
{
    [Required]
    public int Order { get; set; }
    [Required]
    public string Code { get; set; } = null!;
    public string? ParentPlaceHolder { get; set; } = null;
    [Required]
    public  Guid ParentId { get;  set; }
    public string PlaceHolder=>  PlaceHolderUtils.GetPlaceHolder(PlaceHolderType.PRODUCT_COMPONENT, Code, ParentPlaceHolder);
    [Required]
    public string Name { get; set; } = null!;
    public bool Mandatory { get; set; }
    public Guid ComponentId { get;  set; }       
    public string? ConsumptionCalculation { get; set; }
}